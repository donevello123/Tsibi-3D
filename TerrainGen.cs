using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pixel{
    public Vector2? coords;
    public Color color;
}
public class Neighbor{
    public Vector2? _bottomLeft;
    public Vector2? _bottomCenter;
    public Vector2? _bottomRight;
    public Vector2? _centerLeft;
    public Vector2? _centerRight;
    public Vector2? _topLeft;
    public Vector2? _topCenter;
    public Vector2? _topRight;

    public Neighbor(Vector2? bottomLeft = null,
                    Vector2? bottomCenter = null,
                    Vector2? bottomRight = null,
                    Vector2? centerLeft = null,
                    Vector2? centerRight = null,
                    Vector2? topLeft = null,
                    Vector2? topCenter = null,
                    Vector2? topRight = null)
    {
            _bottomCenter = bottomCenter;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
        
            _centerLeft = centerLeft;
            _centerRight = centerRight;

            _topLeft = topLeft;
            _topCenter = topCenter;
            _topRight = topRight;
    }
    public void RemoveAtVector(Vector2 vector){
        if(_bottomLeft == vector)
            _bottomLeft = null;
        if(_bottomCenter == vector)
            _bottomCenter = null;
        if(_bottomRight == vector)
            _bottomRight = null;

        if(_centerRight == vector)
            _centerRight = null;
        if(_centerLeft == vector)
            _centerLeft = null;

        if(_topLeft == vector)
            _topLeft = null;
        if(_topCenter == vector)
            _topCenter = null;
        if(_topRight == vector)
            _topRight = null;    
    }
    public void RemoveBottomRow(){
        _bottomCenter = null;
        _bottomRight = null;
        _bottomLeft = null;
    }
    public void RemoveTopRow(){
        _topCenter = null;
        _topRight = null;
        _topLeft = null;
    }

     public void RemoveCenterRow(){
        _centerLeft = null;
        _centerRight = null;
    }
    public List<Vector2> GetActiveVectors(){
        List<Vector2> list = new List<Vector2>();

         if(_bottomLeft != null)
            list.Add((Vector2)_bottomLeft);
        if(_bottomCenter != null)
            list.Add((Vector2)_bottomCenter);
        if(_bottomRight != null)
            list.Add((Vector2)_bottomRight);

        if(_centerRight != null)
            list.Add((Vector2)_centerRight);
        if(_centerLeft != null)
            list.Add((Vector2)_centerLeft);

        if(_topLeft != null)
            list.Add((Vector2)_topLeft);
        if(_topCenter != null)
            list.Add((Vector2)_topCenter);
        if(_topRight != null)
            list.Add((Vector2)_topRight);   

        return list;
    }
}

public class TerrainGen : MonoBehaviour
{
    // 1. Generate a pixel texture with variable size.
    // 2. Set pixel colors based on terrain generator.
    public const int gridSize = 32;
    public float tileSize = 1;
    public int brushSize = 4;

    public Vector2 startPos;
    public Vector2 endPos;

    private Vector2 currentPos;
    private List<Vector2> baseMap;

    public Color startColor = Color.red;
    public Color endColor = Color.green;
    public Color landColor = Color.white;
    public Color emptyColor = Color.black;
    public Color waterColor = Color.cyan;
    public Color bridgeColor = new Color(100,100,100);
    public string seed = "meSeed";
    private Vector2 moveDirection;
    private void Start() {
        baseMap = new List<Vector2>();
        moveDirection = Vector2.zero;
       // rng = new System.Random(seed.GetHashCode());
    }

    private string DirectionToLiteral(Vector2 vector)
    {
        string y = "";
        string x = "";
        
        switch (vector.y)
        {
            case 1:
                y = "Up";
                break;
            case 0:
                break;
            case -1:
                y = "Down";
                break;
            default:
            break;
        }
         switch (vector.x)
        {
            case 1:
                y = "Right";
                break;
            case 0:
                break;
            case -1:
                y = "Left";
                break;
            default:
            break;
        }

        return x + " " + y;
    }

    public Texture2D BlankTileTexture() {
        System.Random rng = new System.Random(seed.GetHashCode());

        //Clear the previous base map, if one exists
        if(baseMap != null)
            baseMap.Clear();
        
        // Create new texture based on grid size
        Texture2D tex = new Texture2D(gridSize,gridSize); 
        Vector2 tempVector = new Vector2(0,0);
        
        //Set x value for start pixel
        int rs = gridSize/2;
             
        //Fill all pixels with empty colors
        for (int y = 0; y < gridSize; y++){
            for (int x = 0; x < gridSize; x++){
                tex.SetPixel(x,y, emptyColor); 
            }   
        }

        //Generate start pixel at random x location and set starting position. Add pixel to list of pixels in base map
        currentPos = new Vector2(rs, 0);
        tex.SetPixel((int)currentPos.x,(int)currentPos.y,startColor);
        startPos = currentPos;
        baseMap.Add(currentPos);

        //Generate base map
        while(currentPos.y < gridSize - 1)
        {
            switch(currentPos.y){     
            
        //Generate pixels from start to end
                default:
                    Vector2 nextLoc = FilterNeighbors(currentPos, tex, rng);
                    tex.SetPixel((int)nextLoc.x, (int)nextLoc.y,landColor);
                    currentPos = nextLoc;
                break;
            }
            baseMap.Add(currentPos);
        }


        //Color pixels around base map path based on brush size 
        for (int x = 0; x < baseMap.Count; x++){
           DrawCircle(tex,landColor,(int)baseMap[x].x,(int)baseMap[x].y,brushSize);
        }
        
         //Set color of start pixel
        tex.SetPixel((int)baseMap[0].x,0,startColor);

        //Set color of end pixel
        tex.SetPixel((int)baseMap[baseMap.Count-1].x,gridSize-1,endColor);

        
        //Apply the generated map to texture and return the map texture
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }
    private Color Pixel(Texture2D _tex, float x, float y)
    {
        return _tex.GetPixel((int)x,(int)y);
    }

    private bool PixelIsColor(Texture2D _tex, float x, float y, Color color)
    {
        return _tex.GetPixel((int)x,(int)y) == color ? true: false;
     }

    private bool IsWithinTileGrid(int x, int y)
    {
       return x >= 0 && x < gridSize && y>=0 && y<gridSize;
    }
    private Vector2 FilterNeighbors(Vector2 current, Texture2D tex, System.Random rng){
        
        //Retrieve all neighbors
        Neighbor temp = ReturnAllNeighbors(current);

        //Remove bottom row of possible neighbors
        temp.RemoveBottomRow();
        
        if(current.y ==0)
        {
            temp.RemoveCenterRow();
        }

        //Return all neighbors that are not null as a Vector list
        List<Vector2> activeNeighbors = temp.GetActiveVectors();

        //Don't allow a neighbor to move in the same direction twice (unless its in the up direction)
        for (int i = 0; i < activeNeighbors.Count; i++){
            Vector2 tempVector = activeNeighbors[i] - current;
            
            if(tempVector == moveDirection && moveDirection != new Vector2(0,1))
            {
                activeNeighbors.RemoveAt(i);
                continue;
            }
        }
        
        //Don't allow a neighbor to occupy an alraedy occupied location
        for (int i = 0; i < activeNeighbors.Count; i++){
            if(Pixel(tex,activeNeighbors[i].x,activeNeighbors[i].y) != emptyColor)
            {
                activeNeighbors.RemoveAt(i);
                continue;
            }
        }

        for (int i = 0; i < activeNeighbors.Count; i++)
        {
            int x = (int)activeNeighbors[i].x;
            int y = (int)activeNeighbors[i].y;
            Vector2 temp1 = activeNeighbors[i] - current;

            if(Pixel(tex,x+1,y) != emptyColor 
            && Pixel(tex,x,y-1) != emptyColor 
            && Pixel(tex,x+1,y-1) != emptyColor){
                activeNeighbors.RemoveAt(i);
            }else if(Pixel(tex,x-1,y) != emptyColor 
            && Pixel(tex,x,y-1) != emptyColor 
            && Pixel(tex,x-1,y-1) != emptyColor){ 
                activeNeighbors.RemoveAt(i);
            }
           // print("<color=#FF9500>Possible Directions: "+ DirectionToLiteral(temp1) + " at " + activeNeighbors[i] + "</color>");
        }

        //Choose a random direction based on available candidates
        int num = rng.Next(activeNeighbors.Count);
        
        for (int i = 0; i < activeNeighbors.Count; i++){
            if(num == i){
                moveDirection = activeNeighbors[i] - current;
                Debug.Log("Move direction: <color=#00FF44>" + DirectionToLiteral(moveDirection) + "</color>");
                return activeNeighbors[i];
            }   
        }

        //Report if there was an error
        Debug.Log("Couldn't find suitable neighbor.");
        return Vector2.zero;
    }

    private Neighbor ReturnAllNeighbors(Vector2 pixelLoc){
        Neighbor temp;

        switch(pixelLoc.y){
            case 0:
                temp = new Neighbor(//topLeft: new Vector2(pixelLoc.x-1,pixelLoc.y+1),
                                    topCenter: new Vector2(pixelLoc.x,pixelLoc.y+1)
                                    /*topRight: new Vector2(pixelLoc.x+1,pixelLoc.y+1)*/);

            break;

            case gridSize-1:
                temp = new Neighbor(centerLeft: new Vector2(pixelLoc.x-1,pixelLoc.y),
                                    centerRight: new Vector2(pixelLoc.x+1,pixelLoc.y),
                                    bottomCenter: new Vector2(pixelLoc.x,pixelLoc.y-1));
               // Diagonals
               //  temp.Add(new Vector2(pixelLoc.x+1,pixelLoc.y-1));
               //  temp.Add(new Vector2(pixelLoc.x-1,pixelLoc.y-1));

            break;

            default:
                temp = new Neighbor(centerLeft: new Vector2(pixelLoc.x-1,pixelLoc.y),
                                    centerRight: new Vector2(pixelLoc.x+1,pixelLoc.y),
                                    bottomCenter: new Vector2(pixelLoc.x,pixelLoc.y-1),
                                    topCenter: new Vector2(pixelLoc.x,pixelLoc.y+1));

                //Diagonals
               //  temp.Add(new Vector2(pixelLoc.x+1,pixelLoc.y+1));
               //  temp.Add(new Vector2(pixelLoc.x+1,pixelLoc.y-1));
               //  temp.Add(new Vector2(pixelLoc.x-1,pixelLoc.y+1));
               //  temp.Add(new Vector2(pixelLoc.x-1,pixelLoc.y-1));    
            break;              
        }
        return temp;
    }

    public void DrawCircle(Texture2D tex, Color color, int x, int y, int radius = 3)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    tex.SetPixel(u, v, color);

    }
}