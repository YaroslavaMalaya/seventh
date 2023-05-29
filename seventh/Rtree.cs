namespace seventh;

public class CoordinatePair
{
    public double X { get; } // latitude
    public double Y { get; } // longitude
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
        /* the same as
        if (place1 != "")
        {
            Place1 = place1;
        }
        else
        {
            Place1 = "NaN";
        } */
    }
}

public class Rectangle
{
    public CoordinatePair A { get; } // lat_min lon_min
    public CoordinatePair B { get; } // lat_min lon_max
    public CoordinatePair C { get; } // lat_max lon_max
    public CoordinatePair D { get; } // lat_max lon_min

    public Rectangle(double lat_max, double lon_max, double lat_min, double lon_min)
    {
        A = new CoordinatePair(lat_min, lon_min);
        B = new CoordinatePair(lat_min, lon_max);
        C = new CoordinatePair(lat_max, lon_max);
        D = new CoordinatePair(lat_max, lon_min);
    }
}

public class Node
{
    public Rectangle Rect { get; }
    public Node LeftChild { get; set; }
    public Node RightChild { get; set; }
    public List<CoordinatePair> Points { get; set; }
    
    public Node(Rectangle rect, Node left = null, Node right = null, List<CoordinatePair> points = null)
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
    
    public void Build(List<CoordinatePair> points, Node node = null)
    {
        if (node == null)
        {
            var first_rect = MakeRect(points);
            _root = new Node(first_rect);
        }
        else
        {
            _root = node;
        }
        
        if (points.Count <= 10)
        {
            _root.Points = points;
            return;
        }
        
        // сортуємо список щоб знайти елемент по середині і розділити список на дві частини (перша то є менша частина, а дурга то є більша)
        if (_check)
        {
            points.Sort((a, b) => a.X.CompareTo(b.X)); // сортує відносно latitude; a і b це CoordinatePair з points
        }
        else
        {
            points.Sort((a, b) => a.Y.CompareTo(b.Y)); // сортує відносно longitude; a і b це CoordinatePair з points
        }
        _check = !_check; // змінюємо на протилежний 

        var middle = points.Count / 2;
        var left_points = points.GetRange(0, middle); // від нульового елемента до середини
        var right_points = points.GetRange(middle, points.Count - middle); // від середини до останнього
        
        var leftRect = MakeRect(left_points);
        var rightRect = MakeRect(right_points);
        
        _root.LeftChild = new Node(leftRect);
        _root.LeftChild.Points = left_points;
        _root.RightChild = new Node(rightRect);
        _root.RightChild.Points = right_points;

        Build(left_points, _root.LeftChild);
        Build(right_points, _root.RightChild);
    }

    private Rectangle MakeRect(List<CoordinatePair> points)
    {
        double lat_max = 0;
        double lon_max = 0;
        var lat_min = double.MaxValue;
        var lon_min = double.MaxValue;

        foreach (var point in points)
        {
            lat_max = Math.Max(lat_max, point.X);
            lon_max = Math.Max(lon_max, point.Y);
            lat_min = Math.Min(lat_min, point.X);
            lon_min = Math.Min(lon_min, point.Y);
        }

        return new Rectangle(lat_max, lon_max, lat_min, lon_min);
    }
}