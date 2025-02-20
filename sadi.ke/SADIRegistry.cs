﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using LitJson;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SADI.KEPlugin
{
    public class SADIRegistry
    {
        private String endpoint;
        private String graph;

        public SADIRegistry(String endpoint, String graph)
        {
            this.endpoint = endpoint;
            this.graph = graph;
        }

        private static SADIRegistry instance;
        public static SADIRegistry Instance()
        {
            if (instance == null)
            {
                instance = new SADIRegistry("http://biordf.net/sparql", "http://sadiframework.org/registry/");
            }
            return instance;
        }

        public SADIService getService(string serviceURI)
        {
            string query =
                "PREFIX sadi: <http://sadiframework.org/ontologies/sadi.owl#> \r\n" +
                "PREFIX mygrid: <http://www.mygrid.org.uk/mygrid-moby-service#> \r\n" +
                "SELECT * \r\n" +
                "WHERE {\r\n" +
                "   ?serviceURI a sadi:Service . \r\n" +
                "	?serviceURI mygrid:hasServiceNameText ?name . \r\n" +
                "	?serviceURI mygrid:hasServiceDescriptionText ?description . \r\n" +
                "	?serviceURI mygrid:hasOperation ?op . \r\n" +
                "	?op mygrid:inputParameter ?input . \r\n" +
                "	?input a mygrid:parameter . \r\n" +
                "	?input mygrid:objectType ?inputClassURI . \r\n" +
                // instanceQueryPattern is optional here because we're matching by
                // direct type and there's no need for dynamic discovery...
                "   OPTIONAL { ?inputClassURI sadi:instanceQuery ?query } . \r\n" +
                "}";
            query = query.Replace("?serviceURI", "<" + serviceURI + ">");

            foreach (JsonData binding in executeQuery(query))
            {
                SADIService service = new SADIService(
                    getSPARQLBindingAsString(binding, "serviceURI"),
                    getSPARQLBindingAsString(binding, "name"),
                    getSPARQLBindingAsString(binding, "description"),
                    getSPARQLBindingAsString(binding, "inputClassURI"));
                service.inputInstanceQuery = getSPARQLBindingAsString(binding, "query");
                return service;
            }
            throw new ArgumentException(serviceURI + " is not a registered service");
        }

        public int getServiceCount()
        {
            string query =
                "PREFIX sadi: <http://sadiframework.org/ontologies/sadi.owl#> \r\n" +
                "SELECT COUNT(*) \r\n" +
                "WHERE { \r\n" +
                "  ?s a sadi:Service . \r\n" +
                "}";
            return getSPARQLBindingAsInteger(executeQuery(query)[0], "callret-0");
        }

        public ICollection<SADIService> getAllServices(int offset, int limit)
        {
            ICollection<SADIService> services = new List<SADIService>();

            string query =
                "PREFIX sadi: <http://sadiframework.org/ontologies/sadi.owl#> \r\n" +
                "PREFIX mygrid: <http://www.mygrid.org.uk/mygrid-moby-service#> \r\n" +
                "SELECT * \r\n" +
                "WHERE {\r\n" +
                "   ?serviceURI a sadi:Service . \r\n" +
                "	?serviceURI mygrid:hasServiceNameText ?name . \r\n" +
                "	?serviceURI mygrid:hasServiceDescriptionText ?description . \r\n" +
                "	?serviceURI mygrid:hasOperation ?op . \r\n" +
                "	?op mygrid:inputParameter ?input . \r\n" +
                "	?input a mygrid:parameter . \r\n" +
                "	?input mygrid:objectType ?inputClassURI . \r\n" +
                // instanceQuery is not optional here because we only want services
                // whose input instances can be dynamically discovered...
                "   ?inputClassURI sadi:instanceQuery ?query . \r\n" +
                "}";

            foreach (JsonData binding in executeQuery(query))
            {
                SADIService service = new SADIService(
                    getSPARQLBindingAsString(binding, "serviceURI"),
                    getSPARQLBindingAsString(binding, "name"),
                    getSPARQLBindingAsString(binding, "description"),
                    getSPARQLBindingAsString(binding, "inputClassURI"));
                service.inputInstanceQuery = getSPARQLBindingAsString(binding, "query");
                services.Add(service);
            }
            return services;
        }

        public ICollection<SADIService> findServicesByInputClass(ICollection<String> types)
        {
            ICollection<SADIService> services = new List<SADIService>();

            if (types.Count == 0)
            {
                return services;
            }

            string query =
                "PREFIX sadi: <http://sadiframework.org/ontologies/sadi.owl#> \r\n" +
                "PREFIX mygrid: <http://www.mygrid.org.uk/mygrid-moby-service#> \r\n" +
                "SELECT * \r\n" +
                "WHERE {\r\n" +
                "   ?serviceURI a sadi:Service . \r\n" +
                "	?serviceURI mygrid:hasServiceNameText ?name . \r\n" +
                "	?serviceURI mygrid:hasServiceDescriptionText ?description . \r\n" +
                "	?serviceURI mygrid:hasOperation ?op . \r\n" +
                "	?op mygrid:inputParameter ?input . \r\n" +
                "	?input a mygrid:parameter . \r\n" +
                "	?input mygrid:objectType ?inputClassURI . \r\n" +
                // instanceQueryPattern is optional here because we're matching by
                // direct type and there's no need for dynamic discovery...
                "   OPTIONAL { ?inputClassURI sadi:instanceQuery ?query } . \r\n";

            int n=0;
            foreach (string type in types)
            {
                if (++n > 1)
                {
                    query += "	UNION \r\n";
                }
                query += "	{ ?input mygrid:objectType <" + type + "> } \r\n";
                
            }

            query +=
                "}";

            foreach (JsonData binding in executeQuery(query))
            {
                SADIService service = new SADIService(
                    getSPARQLBindingAsString(binding, "serviceURI"),
                    getSPARQLBindingAsString(binding, "name"),
                    getSPARQLBindingAsString(binding, "description"),
                    getSPARQLBindingAsString(binding, "inputClassURI"));
                service.inputInstanceQuery = getSPARQLBindingAsString(binding, "query");
                services.Add(service);
            }
            return services;
        }

        public void addPropertyRestrictions(SADIService service)
        {
            if (service.properties.Count > 0)
            {
                SADIHelper.debug("SADIRegistry", "addPropertyRestrictions called twice on same service", service);
                return;
            }

            String query =
                "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> \r\n" +
                "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> \r\n" +
                "PREFIX owl: <http://www.w3.org/2002/07/owl#> \r\n" +
                "PREFIX sadi: <http://sadiframework.org/ontologies/sadi.owl#> \r\n" +
                "PREFIX mygrid: <http://www.mygrid.org.uk/mygrid-moby-service#> \r\n" +
                "SELECT DISTINCT ?onPropertyURI ?onPropertyLabel ?valuesFromURI ?valuesFromLabel \r\n" +
                "WHERE {\r\n" +
                "	<" + service.uri + "> sadi:decoratesWith ?decoration . \r\n" +
                "	?decoration owl:onProperty ?onPropertyURI . \r\n" +
                "	OPTIONAL { ?onPropertyURI rdfs:label ?onPropertyLabel } .  \r\n" +
                "	OPTIONAL { \r\n" +
                "		?decoration owl:someValuesFrom ?valuesFromURI . \r\n" +
                "		OPTIONAL { ?valuesFromURI rdfs:label ?valuesFromLabel } \r\n" +
                "	} . \r\n" +
                "}";

            foreach (JsonData binding in executeQuery(query))
            {
                service.addProperty(
                    getSPARQLBindingAsString(binding, "onPropertyURI"), 
                    getSPARQLBindingAsString(binding, "onPropertyLabel"), 
                    getSPARQLBindingAsString(binding, "valuesFromURI"), 
                    getSPARQLBindingAsString(binding, "valuesFromLabel"));
            }
        }

        private JsonData executeQuery(string query)
        {
            //SADIHelper.debug("SADIRegistry", "executing query", query);
            NameValueCollection data = new NameValueCollection();
            data.Add("query", query);
            data.Set("format", "JSON");
            if (graph != null)
            {
                data.Set("default-graph-uri", graph);
            }

            WebClient client = new WebClient();
            byte[] response = client.UploadValues(endpoint, data);
            JsonData json = JsonMapper.ToObject(Encoding.UTF8.GetString(response));
            //SADIHelper.debug("SADIRegistry", "query returned", json["results"]["bindings"].ToJson());
            return json["results"]["bindings"];
        }

        private static string getSPARQLBindingAsString(JsonData binding, String variable)
        {
            try
            {
                return binding[variable]["value"].ToString();
            }
            catch
            {
                return null;
            }
        }

        private static int getSPARQLBindingAsInteger(JsonData binding, String variable)
        {
            JsonData value = binding[variable]["value"];
            try
            {
                return value.IsInt ? (int)value : Convert.ToInt32(value.ToString());
            }
            catch
            {
                return 0;
            }
        }
    }
}
