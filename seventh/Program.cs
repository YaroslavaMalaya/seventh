using System.Globalization;
using seventh;

Console.WriteLine("\nEnter latitude, longitude and radius (with space):"); // example 50,4532 30,5183 20
var input = Console.ReadLine().Split(" ");
var lat = double.Parse(input[0].Replace(',', '.'), CultureInfo.InvariantCulture) * Math.PI / 180;
var lon = double.Parse(input[1].Replace(',', '.'), CultureInfo.InvariantCulture);
var radius = double.Parse(input[2].Replace(',', '.'), CultureInfo.InvariantCulture);
var radiusEarth = 6371.032;
var result1 = new List<string>();

// #1
/*foreach (var line in File.ReadAllLines("ukraine_poi.csv"))
{
    var line_el = line.Split(";");
    if (line_el[0] != "")
    {
        var lat2 = double.Parse(line_el[0]) * Math.PI / 180;
        var lon2 = double.Parse(line_el[1]);
        var lat3 = (lat - lat2) * Math.PI / 180;
        var lon3 = (lon - lon2) * Math.PI / 180;
        var haversine_length = 2 * radiusEarth * Math.Asin(Math.Sqrt(Math.Abs(
            Math.Pow(Math.Sin(lat3 / 2), 2) + Math.Cos(lat) * Math.Cos(lat2) * Math.Pow(Math.Sin(lon3 / 2), 2))));

        if (haversine_length <= radius)
        {
            result1.Add(string.Join("; ", line_el[2..].Where(e => e != "")));
        }
    }
}

Console.WriteLine("\nList of locations in the area:");
var count = 1;
if (result1.Count > 0)
{
    foreach (var element in result1)
    {
        Console.WriteLine(count + ". " + element);
        count++;
    }
}
else
{
    Console.WriteLine("No suitable location in this area :(");
}*/

// #2
var allPoints = new List<CoordinatePair>();
foreach (var line2 in File.ReadAllLines("ukraine_poi.csv"))
{
    var lineSplit = line2.Split(";");
    if (lineSplit[0] != "")
    {
        allPoints.Add(new CoordinatePair(double.Parse(lineSplit[0].Replace(',', '.'), CultureInfo.InvariantCulture), 
            double.Parse(lineSplit[1].Replace(',', '.'), CultureInfo.InvariantCulture), 
            lineSplit[2], lineSplit[3], lineSplit[4]));
    }
}

var tree = new Rtree();
tree.Build(allPoints, null);

var initial_point = new CoordinatePair(lat, lon);
var latitudeC = Math.Asin(Math.Sin(lat)*Math.Cos(radius/radiusEarth) +
                      Math.Cos(lat)*Math.Sin(radius/radiusEarth)*Math.Cos(90 * Math.PI / 180)) * 180 / Math.PI; // in degrees 
var latitudeA = Math.Abs(lat -  latitudeC);
var longitudeA = (lon * Math.PI / 180 + Math.Atan2(Math.Sin(Math.PI / 180)*Math.Sin(radius/radiusEarth)*Math.Cos(lat),
    Math.Cos(radius/radiusEarth)-Math.Sin(lat)*Math.Sin(latitudeA))) * 180 / Math.PI; // in degrees 
var longitudeC = lon + Math.Abs(lon - longitudeA);



// form a rectangle for the main point with radius;
var lowLeft = new CoordinatePair(latitudeA, longitudeA);
var upRight = new CoordinatePair(latitudeC, longitudeC);
var mainRectangle = new Rectangle(lowLeft, upRight, radius, initial_point);
var result2 = tree.Find(mainRectangle);

Console.WriteLine("\nList of locations in the area:");
var count = 1;
if (result2.Count > 0)
{
    foreach (var element in result2)
    {
        Console.WriteLine(count + ". " + element);
        count++;
    }
}
else
{
    Console.WriteLine("No suitable location in this area :(");
}
Console.WriteLine('h');



//var longitudeC = (lon * Math.PI / 180 + Math.Atan2(Math.Sin(90 * Math.PI / 180)*Math.Sin(radius/radius_earth)*Math.Cos(lat)Math.Cos(radius/radius_earth)-Math.Sin(lat)*Math.Sin(latitudeC))) * 180 / Math.PI; // in degrees 
//var latitudeA = Math.Asin( Math.Sin(lat)*Math.Cos(radius/radius_earth) + Math.Cos(lat)*Math.Sin(radius/radius_earth)*Math.Cos(Math.PI / 180));
//latitudeA = latitudeA * 180 / Math.PI; // in degrees 