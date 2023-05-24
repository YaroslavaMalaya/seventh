Console.WriteLine("\nEnter latitude, longitude and radius (with space):"); // example 49,06183 22,68685 5
var input = Console.ReadLine().Split(" ");
var lat = double.Parse(input[0]) * Math.PI / 180;
var lon = double.Parse(input[1]);
var radius = double.Parse(input[2]);
var radius_earth = 6371.032;
var result = new List<string>();

foreach (var line in File.ReadAllLines("ukraine_poi.csv"))
{
    var line_el = line.Split(";");
    if (line_el[0] != "")
    {
        var lat2 = double.Parse(line_el[0]) * Math.PI / 180;
        var lon2 = double.Parse(line_el[1]);
        var lat3 = (lat - lat2) * Math.PI / 180;
        var lon3 = (lon - lon2) * Math.PI / 180;
        var haversine_length = 2 * radius_earth * Math.Asin(Math.Sqrt(Math.Abs(
            Math.Pow(Math.Sin(lat3 / 2), 2) + Math.Cos(lat) * Math.Cos(lat2) * Math.Pow(Math.Sin(lon3 / 2), 2))));

        /* варіант з https://www.movable-type.co.uk/scripts/latlong.html (рахує так само)
        var a = Math.Abs(Math.Sin(lat3/2) * Math.Sin(lat3/2) +  Math.Cos(lat) * Math.Cos(lat2) * Math.Sin(lon3/2) * Math.Sin(lon3/2));
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = radius_earth * c; // == haversine_length */
    
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
}