var coordsList, distList, coordsListArr, gDir, cnt, hwttc, map, marker, latlong, marker, markerArray;
markerArray = new Array();
//General scripts

function validate(what) {
    if (what == 1) {
        if (document.getElementById("txtStopNo").value == "") {
            alert("Stop number required.");
            return false;
        }
        if (document.getElementById("txtLocation").value == "") {
            alert("Location required.");
            return false;
        }
        if (!latlong) {
            alert("A marker is required on the map. Click where you need the bus stop.");
            return false;
        }
        return true;
    }

}


//Google Maps scripts and vars
hwttc = new google.maps.LatLng(18.01209505590662, -76.7985588312149);


function detectBrowser() {
    var useragent = navigator.userAgent;
    var mapdiv = document.getElementById("theMap");

    if (useragent.indexOf('iPhone') != -1 || useragent.indexOf('Android') != -1) {
        mapdiv.style.width = '100%';
        mapdiv.style.height = '100%';
    } else {
        mapdiv.style.width = '100%';
        mapdiv.style.height = '500px';
    }
}

function initialize() {
    var myOptions = {
        center: hwttc, zoom: 17, mapTypeId: google.maps.MapTypeId.HYBRID
    };
    map = new google.maps.Map(document.getElementById("theMap"),
            myOptions);
    detectBrowser();

    google.maps.event.addListener(map, "click", function (event) {

        if (!marker) {
            marker = new google.maps.Marker({
                position: event.latLng,
                map: map,
                draggable: true,
                title: String(event.latLng)
            });

            google.maps.event.addListener(marker, "dragend", function () {

                marker.setTitle(String(marker.getPosition()));

            });
            latlong = event.latLng;
        }
        else {
            alert("Marker already exists at " + String(marker.getPosition()));
        }
    });
    
}

function go(where) {

    map.setCenter(where);

}

function getDirections() {
    coordsList = document.getElementById('hidCoordsList').value;
    distList = document.getElementById('hidDistList').value;
    coordsListArr = coordsList.split('|');
    for (cnt = 0; cnt < coordsListArr.length - 1; cnt++) {
        setTimeout("gDir.load('from: ' + coordsListArr[cnt] + ' to: ' + coordsListArr[cnt + 1])", 100);
        setTimeout("var temp = gDir.getDistance().meters", 100);
        setTimeout("distList += temp + ','", 100);
    }
    var delay = cnt * 110;
    setTimeout("document.getElementById('hidDistList').value = distList", delay);
    setTimeout("alert('Click Get Distances')", delay);
}

function showdata() {
    var txthsl = document.getElementById('hidStopsList');
    alert(txthsl.value);
}


//XML scripts and vars

/*function loadXmlDoc(xdocname){
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    xmlhttp.open("GET", xdocname, true);
    xmlhttp.send();
    return xmlhttp.responseXML;
}

function addStop(location, stopNo) {
    alert(marker.getPosition());
    var latlng = marker.getPosition();
    var xdoc = loadXmlDoc("../App_Data/stopsgps.xml");
    var newElem = xdoc.createElement("stop");

    var newStopNo = xdoc.createElement("stopNo");
    newStopNo.setAttribute("Status","new");
    var newSText = xdoc.createTextNode(stopNo);
    newStopNo.appendChild(newText);

    var newLat = xdoc.createElement("latitude");
    var newLText = xdoc.createTextNode(latlng.x);
    newLat.appendChild(newLText);

    var newLng = xdoc.createElement("longitude");
    var newLnText = xdoc.createTextNode(latlng.y);
    newLng.appendChild(newLoText);

    var newLoc = xdoc.createElement("location");
    var newLoText = xdoc.createTextNode(location);
    newLoc.appendChild(newLoText);

    newElem.appendChild(newStopNo);
    newElem.appendChild(nnewLat);
    newElem.appendChild(newLng);
    newElem.appendChild(newLoc);
    xdoc.appendChild(newElem);

}

function ddlShowBuses_OnChange() {
    var lat = 0.0;
    var longi = 0.0;
    var laststop = "";
    var busNum = "";
    var routeNum = "";

    xmldoc = loadxmldoc("/App_Data/busgps.xml");

    if (document.getElementById('ddlShowBuses').value == "Show all buses") {

    }
}*/


/*AJAX Scripts and Vars*/

var xmlHttp;
var requestURL = "http://localhost:3150/Admin/TrackAjax.aspx";
var is_ie = (navigator.userAgent.indexOf("MSIE") >= 0) ? 1 : 0;
var is_ie5 = (navigator.appVersion.indexOf("MSIE 5.5") != -1) ? 1 : 0;
var is_opera = ((navigator.userAgent.indexOf("Opera6") != -1) ||
                (navigator.userAgent.indexOf("Opera/6") != -1)) ? 1 : 0;
var is_netscape = (navigator.userAgent.indexOf("Netscape") >= 0) ? 1 : 0;

function runQuery(str) {
    if (validate("1")) {
        var url = requestURL;
        var radio = document.getElementsByName("type");
        var selected;
        for (var i = 0; i < 2; i++) {
            if (radio[i].checked) {
                selected = radio[i].value;
            }
        }
        xmlHttp = GetXmlHttpObject(ChangeHandler);
        if (str = 1) {
            url += "?query=1&latlong=";
            url += latlong;
            url += "&stopNo=";
            url += txtStopNo.value;
            url += "&location=";
            url += txtLocation.value;
            url += "&type=";
            url += selected;
            xmlHttp_Get(xmlHttp, url);
        }
        //prompt("copy",url);
    }
}

function clearMarkers() {
    for (var cnt = 0; cnt < markerArray.length; cnt++) {
        markerArray[cnt].setMap(null);
    }
}

function ddlStopList_SelectedIndexChanged() {
    if (document.getElementById("ddlStopList").value == "No Stops") {
        clearMarkers();
    }
    else {
        var url = requestURL;
        xmlHttp = GetXmlHttpObject(ChangeHandler);
        url += "?query=2&option=" + document.getElementById("ddlStopList").value;
        xmlHttp_Get(xmlHttp, url);
        setTimeout(function () {
            clearMarkers();
            alert(document.getElementById("addStopResponse").innerHTML);
            var lats = document.getElementsByName("latitude");
            var longs = document.getElementsByName("longitude");
            var stopNo = document.getElementsByName("stopNo");
            var loc = document.getElementsByName("location");
            var type = document.getElementsByName("type");
            for (var cnt = 0; cnt <= document.getElementById("total").value; cnt++) {
                var str = "Stop Number: " + stopNo[cnt].value + "\nLocation: " + loc[cnt].value + "\nType: " + type[cnt].value;
                markerArray[cnt] = new google.maps.Marker({
                    position: new google.maps.LatLng(lats[cnt].value, longs[cnt].value),
                    map: map,
                    title: String(str)
                });
            }
        }, 2000);
    }
    
}


function ChangeHandler() {
    //alert(xmlHttp.readyState)
    if (xmlHttp.readyState ==  4 ||
        xmlHttp.readyState == "complete") {
        //get the results from the callback 
        //alert("THE HTML STRING " + xmlHttp.responseText);
        //populate the innerHTML of the div with the results
        //document.getElementById('temp').innerHTML = xmlHttp.responseText;
        document.getElementById("addStopResponse").innerHTML = xmlHttp.responseText;  //document.getElementById('serverResponse1').value;
    }
    else {
        document.getElementById("addStopResponse").innerHTML =
           "<img src='../images/chasing_school_bus.gif' /></br><b>Loading please wait</b>";
    }

}

function xmlHttp_Get(xmlhttp, url) {
    xmlhttp.open("GET", url, true);
    xmlhttp.send(null);
}

function GetXmlHttpObject(handler) {
    var objXmlHttp = null; //Create the local xmlHTTP object instance 

    //Create the xmlHttp object depending on the browser 
    if (is_ie) {
        //if not IE default to Msxml2 
        var strObjName = (is_ie5) ? "Microsoft.XMLHTTP" :
                                    "Msxml2.XMLHTTP";

        //Create the object 
        try {
            objXmlHttp = new ActiveXObject(strObjName);
            objXmlHttp.onreadystatechange = handler;
        }
        catch (e) {
            //Object creation error 
            alert("Object cannot be created");
            return;
        }
    }
    else if (is_opera) {
        alert("Opera browser");
        return;
    }
    else {
        // other browsers eg mozilla , netscape and safari 
        objXmlHttp = new XMLHttpRequest();
        objXmlHttp.onload = handler;
        objXmlHttp.onerror = handler;
    }

    //Return the instantiated object 
    return objXmlHttp;
}


/*End Ajax*/