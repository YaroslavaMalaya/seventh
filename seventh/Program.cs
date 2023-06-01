using System.Globalization;
using seventh;

Console.WriteLine("\nEnter latitude, longitude and radius (with space):"); // example 49,06183 22,68685 2
var input = Console.ReadLine().Split(" ");
var lat = double.Parse(input[0], CultureInfo.InvariantCulture) * Math.PI / 180;
var lon = double.Parse(input[1], CultureInfo.InvariantCulture);
var radius = double.Parse(input[2]);
var radiusEarth = 6371.032;
var result = new List<string>();

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
            result.Add(string.Join("; ", line_el[2..].Where(e => e != "")));
        }
    }
}

Console.WriteLine("\nList of locations in the area:");
var count = 1;
if (result.Count > 0)
{
    foreach (var element in result)
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

// створити клас для точок прямокутника
// break коли менше 10 точок в прямокутнику
// пошук потоібних точок за перетином прямокутників 
// спускатися по дереву(по нащадкам) і переіряти чи входить в цей прямокутник наша точка(задана) - рекурсивно 
// потім пройтись по кожній точці з прямокутника і додати у список

var allPoints = new List<CoordinatePair>();
foreach (var line2 in File.ReadAllLines("ukraine_poi.csv"))
{
    var lineSplit = line2.Split(";");
    if (lineSplit[0] != "")
    {
        allPoints.Add(new CoordinatePair(double.Parse(lineSplit[0], CultureInfo.InvariantCulture), 
            double.Parse(lineSplit[1], CultureInfo.InvariantCulture), 
            lineSplit[2], lineSplit[3], lineSplit[4]));
    }
}

var tree = new Rtree();
tree.Build(allPoints, null);


// I'm not sure about it
var latitudeC = Math.Asin(Math.Sin(lat)*Math.Cos(radius/radiusEarth) +
                      Math.Cos(lat)*Math.Sin(radius/radiusEarth)*Math.Cos(90 * Math.PI / 180)) * 180 / Math.PI; // in degrees 
var latitudeA = Math.Abs(lat -  latitudeC);
var longitudeA = (lon * Math.PI / 180 + Math.Atan2(Math.Sin(Math.PI / 180)*Math.Sin(radius/radiusEarth)*Math.Cos(lat),
    Math.Cos(radius/radiusEarth)-Math.Sin(lat)*Math.Sin(latitudeA))) * 180 / Math.PI; // in degrees 
var longitudeC = lon + Math.Abs(lon - longitudeA);

// form a rectangle for the main point with radius;
var lowLeft = new CoordinatePair(latitudeA, longitudeA);
var upRight = new CoordinatePair(latitudeC, longitudeC);
var mainRectangle = new Rectangle(lowLeft, upRight);

//var longitudeC = (lon * Math.PI / 180 + Math.Atan2(Math.Sin(90 * Math.PI / 180)*Math.Sin(radius/radius_earth)*Math.Cos(lat)Math.Cos(radius/radius_earth)-Math.Sin(lat)*Math.Sin(latitudeC))) * 180 / Math.PI; // in degrees 
//var latitudeA = Math.Asin( Math.Sin(lat)*Math.Cos(radius/radius_earth) + Math.Cos(lat)*Math.Sin(radius/radius_earth)*Math.Cos(Math.PI / 180));
//latitudeA = latitudeA * 180 / Math.PI; // in degrees 