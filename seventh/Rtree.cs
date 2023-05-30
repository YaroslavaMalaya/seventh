namespace seventh;

public class CoordinatePair
{
    public double X { get; set; } // latitude
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
    public CoordinatePair B { get; set; } // lat_min lon_max
    public CoordinatePair C { get; } // lat_max lon_max
    public CoordinatePair D { set; get; } // lat_max lon_min
    public double Height { get; set; }
    public double Length { get; set; }

    public Rectangle(double latMin, double lonMax) // not sure if we need this
    {
        A = new CoordinatePair(latMin, lonMax - Height);
        B = new CoordinatePair(latMin, lonMax);
        C = new CoordinatePair(latMin + Length, lonMax);
        D = new CoordinatePair(latMin + Length, lonMax - Height);
    }
    
    public Rectangle(CoordinatePair lowLeft, CoordinatePair upRight)
    {
        A = lowLeft;
        C = upRight;
        
        B = new CoordinatePair(A.X, C.Y);
        D = new CoordinatePair(C.X, A.Y);
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
    
    public void Build(List<CoordinatePair> points, Node node)
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
        
        // сортуємо список щоб знайти елемент по середині і
        // розділити список на дві частини (перша то є менша частина, а друга то є більша)
        if (_check)
        {
            points.Sort((a, b) => a.X.CompareTo(b.X)); 
            // сортує відносно latitude; a і b це CoordinatePair з points
        }
        else
        {
            points.Sort((a, b) => a.Y.CompareTo(b.Y)); 
            // сортує відносно longitude; a і b це CoordinatePair з points
        }
        _check = !_check; // змінюємо на протилежний 

        var middle = points.Count / 2;
        var leftPoints = points.GetRange(0, middle); // від нульового елемента до середини
        var rightPoints = points.GetRange(middle, points.Count - middle); // від середини до останнього

        // still have to come up with this part
        // as when we divide left/right we change one vertices
        // and when upper/lower other
        
        //var A_new = node.Rect.A;
        //A_new.X = right_points[0].X;

        // not sure about this part either
        var leftRect = new Rectangle(node.Rect.A, node.Rect.B);
        
        var rightRect = MakeRect(rightPoints);
        
        _root.LeftChild = new Node(leftRect)
        { Points = leftPoints };
        _root.RightChild = new Node(rightRect)
        { Points = rightPoints };

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