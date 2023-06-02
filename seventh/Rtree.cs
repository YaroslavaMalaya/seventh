namespace seventh;

public class CoordinatePair
{
    public double X { get; set; } // latitude
    public double Y { get; set; } // longitude
    public string? Place1 { get; }
    public string Place2 { get; }
    public string Name { get; }
    public string Address { get; }
    
    public CoordinatePair(double x, double y, string place1 = null, string place2 = null, string name = null, string address = null)
    {
        X = x;
        Y = y;
        Place1 = place1 != "" ? place1 : "NaN"; 
        Place2 = place2 != "" ? place2 : "NaN"; 
        Name = name != "" ? place1 : "NaN"; 
        Address = address != "" ? place1 : "NaN";
    }
}

public class Rectangle
{
    public CoordinatePair A { get; } // lat_min lon_min
    public CoordinatePair C { get; } // lat_max lon_max
    public double? Radius { get; }
    public CoordinatePair? Center { get; set; }

    public Rectangle(CoordinatePair lowLeft, CoordinatePair upRight, double? radius = null, CoordinatePair? center = null)
    {
        A = lowLeft;
        C = upRight;
        Radius = radius;
        Center = center;
        //B = new CoordinatePair(A.X, C.Y);
        //D = new CoordinatePair(C.X, A.Y);
    }
    
    public bool IfIntersect(Rectangle other)
    {
        var a1 = this.A;
        var c1 = this.C;

        var a2 = other.A;
        var c2 = other.C;

        if ((a2.Y > c1.Y || a1.Y > c2.Y) // one above other
            || (a2.X > c1.X || a1.X > c2.X)) // side by side (left or right)
        {
            return false;
        }
        return true;
    }
    
}

public class Node
{
    public Rectangle Rect { get; }
    public Node LeftChild { get; set; }
    public Node RightChild { get; set; }
    public List<CoordinatePair> Points { get; set; }
    
    public Node(Rectangle rect, List<CoordinatePair> points = null)
    {
        Rect = rect;
        Points = points;
    }
}

public class Rtree
{
    public Node _root;
    //private Node _current;
    private bool _check; // true - розбиття по Х; false - розбиття по Y;
    public List<CoordinatePair> result = new List<CoordinatePair>();

    public void Build(List<CoordinatePair> points, Node node)
    {
        if (node == null)
        {
            var first_rect = MakeRect(points);
            _root = new Node(first_rect, points: points);
             node = _root;
        }
        
        if (points.Count <= 10)
        {
            node.Points = points;
            return;
        }
        
        List<CoordinatePair> leftPoints;
        List<CoordinatePair> rightPoints;
        CoordinatePair newA;
        CoordinatePair newC;
        int middle;

        if (_check)
        {
            points.Sort((a, b) => a.X.CompareTo(b.X)); // сортує відносно latitude; a і b це CoordinatePair з points
            take();
            newA = new CoordinatePair(points[middle].X, node.Rect.A.Y);
            newC = new CoordinatePair(points[middle].X, node.Rect.C.Y);
        }
        else
        {
            points.Sort((a, b) => a.Y.CompareTo(b.Y)); // сортує відносно longitude; a і b це CoordinatePair з points
            take();
            newA = new CoordinatePair(node.Rect.A.X, points[middle].Y);
            newC = new CoordinatePair(node.Rect.C.X, points[middle].Y);
        }

        void take()
        {
            middle = points.Count / 2;
            leftPoints = points.GetRange(0, middle + 1);
            rightPoints = points.GetRange(middle, points.Count - middle);
        }

        var leftRect = new Rectangle(node.Rect.A, newC);
        var rightRect = new Rectangle(newA, node.Rect.C);

        node.LeftChild = new Node(leftRect);
        node.RightChild = new Node(rightRect, points: rightPoints);
        
        _check = !_check;
        Build(leftPoints, node.LeftChild);
        Build(rightPoints, node.RightChild);
        
        if (node.LeftChild == null && node.RightChild == null)
        {
            node.Points = points;
        }
        else
        {
            node.Points = null;
        }
        
    }

    public List<CoordinatePair> Find(Rectangle mainRect, Node node = null, List<CoordinatePair> res = null)
    {

        if (res is null)
        {
            res = new List<CoordinatePair>();
        }
        
        if (node is null)
        {
            node = _root;
        }
        
        //  check if there are points in the rectangle of the root or not
        if (!node.Rect.IfIntersect(mainRect))
        {
            return res;
        }

        if (node.LeftChild is null && node.RightChild is null)
        {
            foreach (var point in node.Points)
            {
                var lat = mainRect.Center.X * Math.PI / 180;
                var lon = mainRect.Center.Y * Math.PI / 180;   
                var lat2 = point.X  * Math.PI / 180;
                var lon2 = point.Y * Math.PI / 180;
                var lat3 = (lat - lat2);
                var lon3 = (lon - lon2);
                var haversine_length = 2 * 6371.032 * Math.Asin(Math.Sqrt(Math.Abs(
                        Math.Pow(Math.Sin(lat3 / 2), 2) + Math.Cos(lat) * Math.Cos(lat2) * Math.Pow(Math.Sin(lon3 / 2), 2))));

                if (haversine_length <= mainRect.Radius)
                {
                    res.Add(point);
                }
            }
        }
        else
        {
            res.Concat(Find(mainRect, node.LeftChild, res));
            res.Concat(Find(mainRect, node.RightChild, res));
            // res = Find(mainRect, node.RightChild, res);
        }

        return res;
    }
    
    private Rectangle MakeRect(List<CoordinatePair> points)
    {
        var listX = new List<double>();
        var listY = new List<double>();
        points.ForEach(pair => listX.Add(pair.X));
        points.ForEach(pair => listY.Add(pair.Y));

        var latMin = listX.Min();
        var latMax = listX.Max();
        var lonMin = listY.Min();
        var lonMax = listY.Max();

        return new Rectangle(new CoordinatePair(latMin, lonMin), 
            new CoordinatePair(latMax, lonMax));
    }
}


// double coordinateX;
// double coordinateY;
// static void Swap(double x, double y)
// {
//     (x, y) = (y, x);
// }