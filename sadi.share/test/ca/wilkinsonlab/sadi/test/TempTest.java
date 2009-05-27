package ca.wilkinsonlab.sadi.test;

import java.util.List;
import java.util.Map;

import ca.wilkinsonlab.sadi.pellet.PelletClient;

/**
 * This class executes a single example query and reports the results.
 * Use this for tracking down problems in specific queries, since Eclipse
 * apparently runs all JUnit tests even if you ask it to just run one of
 * them.
 * @author Luke McCarthy
 */
public class TempTest
{
	public static void main(String[] args)
	{
		String query = ExampleQueries.getQueryByHtmlListIndex(1);
		
//		query = "PREFIX pred: <http://es-01.chibi.ubc.ca/~benv/predicates.owl#> " +
//				"SELECT ?term " +
//				"WHERE { " +
//					"<http://biordf.net/moby/UniProt/A2ABK8> pred:hasGOTerm ?term " +
//				"}";
		
		List<Map<String, String>> results = new PelletClient().synchronousQuery(query);
		for (Map<String, String> binding: results)
			System.out.println(binding);
	}
}