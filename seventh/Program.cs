using System.Diagnostics;
using System.Globalization;
using seventh;
var radiusEarth = 6371.032;

while(true)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine("\nEnter latitude, longitude and radius (with space):"); // example 48,5378 37,69629 0.5
    Console.ForegroundColor = ConsoleColor.White;
    var input = Console.ReadLine().Split(" ");
    var lat = double.Parse(input[0].Replace(',', '.'), CultureInfo.InvariantCulture) * Math.PI / 180;
    var lon = double.Parse(input[1].Replace(',', '.'), CultureInfo.InvariantCulture) * Math.PI / 180;
    var radius = double.Parse(input[2].Replace(',', '.'), CultureInfo.InvariantCulture);

    // #1
    var sw1 = new Stopwatch();
    sw1.Start();
    var result1 = new List<List<string>>();
    foreach (var line in File.ReadAllLines("ukraine_poi.csv"))
    {
        var line_el = line.Split(";");
        if (line_el[0] != "")
        {
            var lat2 = double.Parse(line_el[0].Replace(',', '.'), CultureInfo.InvariantCulture) * Math.PI / 180;
            var lon2 = double.Parse(line_el[1].Replace(',', '.'), CultureInfo.InvariantCulture) * Math.PI / 180;
            var lat3 = (lat2 - lat);
            var lon3 = (lon2 - lon);
            var haversine_length = 2 * radiusEarth * Math.Asin(Math.Sqrt(Math.Abs(
                Math.Pow(Math.Sin(lat3 / 2), 2) + Math.Cos(lat) * Math.Cos(lat2) * Math.Pow(Math.Sin(lon3 / 2), 2))));

            if (haversine_length <= radius)
            {
                var point = new List<string>();
                point.Add(string.Join("; ", line_el[2..].Select(e => string.IsNullOrEmpty(e) ? "NaN" : e)));
                result1.Add(point);
            }
        }
    }

    Print(result1);
    sw1.Stop();
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"Elapsed time: {sw1.Elapsed}");
    Console.ForegroundColor = ConsoleColor.White;


    // #2
    var sw2 = new Stopwatch();
    sw2.Start();
    var allPoints = new List<CoordinatePair>();
    foreach (var line2 in File.ReadAllLines("ukraine_poi.csv"))
    {
        var lineSplit = line2.Split(";");
        if (lineSplit[0] != "")
        {
            allPoints.Add(new CoordinatePair(double.Parse(lineSplit[0].Replace(',', '.'), CultureInfo.InvariantCulture),
                double.Parse(lineSplit[1].Replace(',', '.'), CultureInfo.InvariantCulture), lineSplit[2], lineSplit[3],
                lineSplit[4], lineSplit[5]));
        }
    }

    var tree = new Rtree();
    tree.Build(allPoints);
    sw1.Stop();
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"\nElapsed time (for building): {sw2.Elapsed}");
    Console.ForegroundColor = ConsoleColor.White;


    var sw3 = new Stopwatch();
    sw3.Start();
    var initial_point = new CoordinatePair(lat, lon);
    var latC1 = Latitude(lat, 90, radius);
    var longC1 = longitude(lon, lat, latC1, 90, radius);
    var latitudeC = Latitude(latC1, 0, radius);
    var longitudeC = longitude(longC1, latC1, latitudeC, 0, radius) * 180 / Math.PI;
    latitudeC *= 180 / Math.PI;
    var latA = Latitude(lat, -90, radius);
    var longA = longitude(lon, lat, latA, -90, radius);
    var latitudeA = Latitude(latA, 180, radius);
    var longitudeA = longitude(longA, latA, latitudeA, 180, radius) * 180 / Math.PI;
    latitudeA *= 180 / Math.PI;
    // form a rectangle for the main point (with radius);
    var lowLeft = new CoordinatePair(latitudeA, longitudeA);
    var upRight = new CoordinatePair(latitudeC, longitudeC);
    var mainRectangle = new Rectangle(lowLeft, upRight, radius, initial_point);

    var result2 = tree.Find(mainRectangle);
    Print(result2);
    sw3.Stop();
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"Elapsed time (for finding): {sw3.Elapsed}");
    Console.ForegroundColor = ConsoleColor.White;
}



double Latitude(double lat, double degree, double radius)
{
    return Math.Asin(Math.Sin(lat) * Math.Cos(radius / radiusEarth) +
                     Math.Cos(lat) * Math.Sin(radius / radiusEarth) * Math.Cos(degree * Math.PI / 180));
}

double longitude(double lon, double lat, double latThis, double degree, double radius)
{ 
    return lon + Math.Atan2(Math.Sin(degree * Math.PI / 180) * Math.Sin(radius/radiusEarth) * Math.Cos(lat),
        Math.Cos(radius/radiusEarth) - Math.Sin(lat)*Math.Sin(latThis));
}

void Print(List<List<string>> results)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine("\nList of locations in the area:");
    Console.ForegroundColor = ConsoleColor.White;
    var i = 1;
    if (results.Count > 0)
    {
        foreach (var element in results)
        {
            Console.WriteLine($"{i++}. {string.Join("; ", element)}");
        }
    }
    else
    {
        Console.WriteLine("No suitable location in this area :(");
    }
}