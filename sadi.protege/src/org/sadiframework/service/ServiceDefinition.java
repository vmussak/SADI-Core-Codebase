/**
 * 
 */
package org.sadiframework.service;

/**
 * @author Eddie Kawas
 * 
 */
public class ServiceDefinition {

	private String name = "";
	private String authority = "";
	private String serviceType = "";
	private String inputClass = "";
	private String outputClass = "";
	private String description = "";
	private String uniqueID = "";
	private boolean authoritative = false;
	private String provider = "";
	private String serviceURI = "";
	private String endpoint = "";
	private String signatureURL = "";
	private boolean async = false;

	/**
	 * Default constructor
	 */
	public ServiceDefinition() {
		this("");
	}

	/**
	 * @param name
	 *            the name of our service
	 */
	public ServiceDefinition(String name) {
		super();
		this.name = name;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name == null ? "" : name.trim();
	}

	public String getAuthority() {
		return authority;
	}

	public void setAuthority(String authority) {
		this.authority = authority == null ? "" : authority.trim();
	}

	public String getServiceType() {
		return serviceType;
	}

	public void setServiceType(String serviceType) {
		this.serviceType = serviceType == null ? "" : serviceType.trim();
	}

	public String getInputClass() {
		return inputClass;
	}

	public void setInputClass(String inputClass) {
		this.inputClass = inputClass == null ? "" : inputClass.trim();
	}

	public String getOutputClass() {
		return outputClass;
	}

	public void setOutputClass(String outputClass) {
		this.outputClass = outputClass == null ? "" : outputClass.trim();
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public String getUniqueID() {
		return uniqueID;
	}

	public void setUniqueID(String uniqueID) {
		this.uniqueID = uniqueID == null ? "" : uniqueID.trim();
	}

	public boolean isAuthoritative() {
		return authoritative;
	}

	public void setAuthoritative(boolean authoritative) {
		this.authoritative = authoritative;
	}

	public String getProvider() {
		return provider;
	}

	public void setProvider(String provider) {
		this.provider = provider == null ? "" : provider.trim();
	}

	public String getServiceURI() {
		return serviceURI;
	}

	public void setServiceURI(String serviceURI) {
		this.serviceURI = serviceURI == null ? "" : serviceURI.trim();
	}

	public String getEndpoint() {
		return endpoint;
	}

	public void setEndpoint(String endpoint) {
		this.endpoint = endpoint == null ? "" : endpoint.trim();
		// set the signatureURL to be endpoint if it is not defined
		if (getSignatureURL() == null || getSignatureURL().trim().equals(""))
		    setSignatureURL(endpoint);
		// set the Unique ID to be endpoint if it is not defined
		if (getUniqueID() == null || getUniqueID().trim().equals(""))
		    setUniqueID(endpoint);
		// set the service uri to be the endpoint if it is not defined
		if (getServiceURI() == null || getServiceURI().trim().equals(""))
		    setServiceURI(endpoint);
		
	}

	public String getSignatureURL() {
		return signatureURL;
	}

	public void setSignatureURL(String signatureURL) {
		this.signatureURL = signatureURL == null ? "" : signatureURL.trim();
	}

	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder();
		String newline = System.getProperty("line.separator");
		sb.append("# This is a SADI service configuration file" + newline + newline);
		sb.append("# If you want to use the characters '#', ',', or '='" + newline);
		sb.append("# you can either escape them with \\ OR" + newline);
		sb.append("# enclose the whole string in double quotes." + newline);
		sb.append("# New line characters are not supported" + newline);
		sb.append("# dont change the service name! if you find that you need to modify" + newline);
		sb.append("# the service name, regenerate a new file and remove this one!" + newline);
		sb.append( String.format("ServiceName = %s", getName()) + newline);
		sb.append("#modify this value only if you have NOT generated" + newline + "#your service implementation!" + newline + newline);
		sb.append( String.format("Authority = %s", getAuthority()) + newline);
		sb.append("#modify these values as you see fit!" + newline);
		sb.append( String.format("ServiceType = \"%s\"", getServiceType()) + newline);
		sb.append( String.format("InputClass = \"%s\"", getInputClass()) + newline);
		sb.append( String.format("OutputClass = \"%s\"", getOutputClass()) + newline);
		sb.append( String.format("Description = \"%s\"", getDescription()) + newline);
		sb.append( String.format("UniqueIdentifier = %s", getUniqueID()) + newline);
		sb.append( String.format("Provider = %s", getProvider()) + newline);
		sb.append( String.format("Authoritative = %s", isAuthoritative()) + newline);
		sb.append( String.format("ServiceURI = %s", getServiceURI()) + newline);
		sb.append( String.format("URL = %s", getEndpoint()) + newline);
		sb.append( String.format("SignatureURL = %s", getSignatureURL()) + newline);
		
		return sb.toString();
	}

    /**
     * @param async the async to set
     */
    public void setAsync(boolean async) {
        this.async = async;
    }

    /**
     * @return the async
     */
    public boolean isAsync() {
        return async;
    }

}
