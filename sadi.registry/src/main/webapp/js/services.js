google.load("jquery", "1.3.2");
google.setOnLoadCallback(function() {
	jQuery.getScript("js/expandTable.js", function() {
		$("#services-table").expandTable(function(rowBody) {
			var row = rowBody.parent("tr");
			var previous = row.prev("tr");
			if (previous.hasClass("even")) {
				row.addClass("even");
			} else if (previous.hasClass("odd")){
				row.addClass("odd");
			}
			var serviceURI = previous.attr("id");
			rowBody.load("service.jsp", {"serviceURI" : serviceURI});
		});
	});
//	$("#register-submit").click(function() {
//		alert($("url-input").value());
//		return false;
//	});
});