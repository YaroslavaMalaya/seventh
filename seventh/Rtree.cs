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

    public Rectangle(CoordinatePair lowLeft, CoordinatePair upRight)
    {
        A = lowLeft;
        C = upRight;
        //B = new CoordinatePair(A.X, C.Y);
        //D = new CoordinatePair(C.X, A.Y);
    }
    
}

public class Node
{
    public Rectangle Rect { get; }
    public Node LeftChild { get; set; }
    public Node RightChild { get; set; }
    public List<CoordinatePair> Points { get; set; }
    
    public Node(Rectangle rect, Node left, Node right, List<CoordinatePair> points = null)
    {
        Rect = rect;
        LeftChild = left;
        RightChild = right;
        Points = points;
    }
}

public class Rtree
{
    private Node _root;
    private bool _check; // true - розбиття по Х; false - розбиття по Y;
    
    public void Build(List<CoordinatePair> points, Node node)
    {
        if (node == null)
        {
            var first_rect = MakeRect(points);
            node = new Node(first_rect, null, null);
        }
        _root = node;

        if (points.Count <= 10)
        {
            _root.Points = points;
            return;
        }
        
        List<CoordinatePair> leftPoints;
        List<CoordinatePair> rightPoints;
        CoordinatePair new_A;
        CoordinatePair new_C;

        if (_check)
        {
            points.Sort((a, b) => a.X.CompareTo(b.X)); // сортує відносно latitude; a і b це CoordinatePair з points
            
            var middle = points.Count / 2;
            leftPoints = points.GetRange(0, middle + 1);
            rightPoints = points.GetRange(middle, points.Count - middle);
            
            new_A = new CoordinatePair(points[middle].X, _root.Rect.A.Y);
            new_C = new CoordinatePair(points[middle].X, _root.Rect.C.Y);

        }
        else
        {
            points.Sort((a, b) => a.Y.CompareTo(b.Y)); // сортує відносно longitude; a і b це CoordinatePair з points
            
            var middle = points.Count / 2;
            leftPoints = points.GetRange(0, middle + 1);
            rightPoints = points.GetRange(middle, points.Count - middle); 
            
            new_A = new CoordinatePair(_root.Rect.A.X, points[middle].Y);
            new_C = new CoordinatePair(_root.Rect.C.Y, points[middle].Y);

        }
        
        var leftRect = new Rectangle(_root.Rect.A, new_C);
        var rightRect = new Rectangle(new_A, _root.Rect.A);

        _root.LeftChild = new Node(leftRect, null, null)
        {
            Points = leftPoints
        };
        _root.RightChild = new Node(rightRect, null, null)
        {
            Points = rightPoints
        };
            
        _check = !_check;
        Build(leftPoints, _root.LeftChild);
        Build(rightPoints, _root.RightChild);
    }

    private Rectangle MakeRect(List<CoordinatePair> points)
    {
        var listX = new List<double>();
        var listY = new List<double>();
        points.ForEach(pair => listX.Add(pair.X));
        points.ForEach(pair => listY.Add(pair.Y));

        var lat_min = listX.Min();
        var lat_max = listX.Max();
        var lon_min = listY.Min();
        var lon_max = listY.Max();

        return new Rectangle(new CoordinatePair(lat_min, lon_min), 
            new CoordinatePair(lat_max, lon_max));
    }
}