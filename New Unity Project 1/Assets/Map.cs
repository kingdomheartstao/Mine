using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public struct TerrainData
{
    public string name;
    public int leve;
    public int rate;

    public TerrainData(string name, int leve, int rate){
        this.name = name;
        this.leve = leve;
        this.rate = rate;
    }
}

public static class Tables
{
    public static Dictionary<string, TerrainData> TerrainTable = new Dictionary<string, TerrainData>
    {
        { "Dust", new TerrainData("dust", 0, 5)},
        { "Rock", new TerrainData("rock", 0, 3)},
        { "Gold", new TerrainData("gold", 0, 2)},
        { "Lava", new TerrainData("lava", 0, 2)},
        { "Empty", new TerrainData("empty", 0, 1)}
    };
}

public struct Index
{
    public int x;
    public int y;
    public Index(int xx, int yy) { x = xx; y = yy; }

    public Index(Index p) {x = p.x; y = p.y;}

    public Index Mid(Index a){
        return new Index((a.x + x) / 2, (a.y + y) / 2);
    }

    public static Index operator + (Index a, Index b) 
    { return new Index(a.x + b.x, a.y + b.y); }

    public static bool operator == (Index a, Index b) { return a.Equals(b); }
    public static bool operator != (Index a, Index b) { return !a.Equals(b); }
}

public class Map : MonoBehaviour {


    enum SideType
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        None
    };


    public class Cell {
        int v;
        Vector3 pos;
    };

    struct Diamond
    {
        public Index top;
        public Index bottom;
        public Index left;
        public Index right;

        public Diamond(Index top, Index bottom, Index left,
            Index right)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }
    }

    struct Square
    {
        public Index top_left;
        public Index top_right;
        public Index bottom_left;
        public Index bottom_right;

        public Square(Index top_left,Index top_right,
            Index bottom_left,Index bottom_right)
        {
            this.top_left = top_left;
            this.top_right = top_right;
            this.bottom_left = bottom_left;
            this.bottom_right = bottom_right;
        }
    }

    public float randomRange;
    public int TerrainHeightExtent;
    public int BaseValue;
    public int CoarseH;
    public int sizeIndex;
    public int size;

    int mIndex;
    int count;
    int mMaxHeightValue;
    int mMinHeightvalue;
    float mCoarseConstant;
    float mRandomRange;
    int[,] map;
    List<Square> squareList = new List<Square>();
    List<Diamond> diamondList = new List<Diamond>();

    public void FractalTerrainGenerator(int index, int baseValue, float H, int extent){

    }

    float GetRandomValue()
    {
        float random = ((Random.Range(0,mRandomRange)) - (mRandomRange / 2)) * TerrainHeightExtent;

        return random;
    }


    void InitGenerator()
    {
        count = 0;
        mIndex = sizeIndex;
        mRandomRange = randomRange;
        mCoarseConstant = Mathf.Pow(0.5f,CoarseH);
        mMaxHeightValue = 0;
        mMinHeightvalue = 0;
        size = Mathf.RoundToInt(Mathf.Pow(2, mIndex)) + 1;
        squareList.Clear();
        diamondList.Clear();
        Index top_left = new Index(0, 0);
        Index top_right = new Index(size - 1, 0);
        Index bottom_left = new Index(0, size - 1);
        Index bottom_right = new Index(size - 1, size - 1);
        Square square = new Square(top_left, top_right, bottom_left, bottom_right);
        squareList.Add(square);
        map = new int[size,size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i,j] = -1;
            }
        }
        map[size - 1, 0] = map[0, 0] = map[0, size - 1] = map[size - 1, size - 1] = BaseValue;
        
    }

    void CoreCalculate()
    {
        count = 1;
        while (mIndex > 0)
        {
            CalculateDiamondPhase();
            CalculateSquarePhase();
            mRandomRange *= mCoarseConstant;
            count++;
            mIndex--;
        }
    }



    void CalculateDiamondPhase()
    {
        diamondList.Clear();
        foreach (Square square in squareList)
        {
            Index top;
            Index bottom;
            Index left;
            Index right;
            Index center = new Index((square .top_left.x + square.top_right.x)/2,
                (square.top_left.y + square.bottom_left.y)/2);
            int sideLen = square.top_right.x - square.top_left.x;
            //set Square's center
            float val =
                (map[square.top_left.x,square.top_left.y]
                + map[square.top_right.x,square.top_right.y]
                + map[square.bottom_left.x,square.bottom_left.y]
                + map[square.bottom_right.x,square.bottom_right.y]) / 4
                + GetRandomValue();
            //val = val > maxLeve ? maxLeve : val; 
            SetMaxHeightValue(val);
            SetMinHeightvalue(val);
            map[center.x,center.y] = Mathf.RoundToInt(val);
            Diamond diamond;
            //up
            top = new Index(center.x, center.y - sideLen);
            bottom = center;
            left = square.top_left;
            right = square.top_right;
            diamond = new Diamond(top, bottom, left, right);
            diamondList.Add(diamond);
            //down
            top = center;
            bottom = new Index(center.x, center.y + sideLen);
            left = square.bottom_left;
            right = square.bottom_right;
            diamond = new Diamond(top, bottom, left, right);
            diamondList.Add(diamond);
            //left
            top = square.top_left;
            bottom = square.bottom_left;
            left = new Index(center.x - sideLen, center.y);
            right = center;
            diamond = new Diamond(top, bottom, left, right);
            diamondList.Add(diamond);
            //right
            top = square.top_right;
            bottom = square.bottom_right;
            left = center;
            right = new Index(center.x + sideLen, center.y);
            diamond = new Diamond(top, bottom, left, right);
            diamondList.Add(diamond);
        }
    }

    void CalculateSquarePhase()
    {
        squareList.Clear();
        foreach (Diamond diamond in diamondList)
        {
            int len = diamond.right.x - diamond.left.x;
            float val = 0;
            Index center = new Index(diamond.left.x + len/2, diamond.top.y + len/2);
            if (diamond.left.x >= 0)
                val += map[diamond.left.x,diamond.left.y];
            if(diamond.right.x <= size - 1)
                val += map[diamond.right.x,diamond.right.y];
            if(diamond.top.y >= 0)
                val += map[diamond.top.x,diamond.top.y];
            if(diamond.bottom.y <= size - 1)
                val += map[diamond.bottom.x,diamond.bottom.y];
            val /= 4;
            val += GetRandomValue();
            //val = val > maxLeve ? maxLeve : val; 
            SetMaxHeightValue(val);
            SetMinHeightvalue(val);
            map[center.x,center.y] = Mathf.RoundToInt(val);
            Square square_TL;
            Square square_TR;
            Square square_BL;
            Square square_BR;
            square_TL = new Square(new Index(diamond.left.x,diamond.top.y),diamond.top,
                                diamond.left, new Index(diamond.top.x,diamond.left.y));
            square_TR = new Square(diamond.top, new Index(diamond.right.x, diamond.top.y),
                                new Index(diamond.top.x,diamond.right.y), diamond.right);
            square_BL = new Square(diamond.left, new Index(diamond.bottom.x, diamond.left.y),
                                new Index(diamond.left.x,diamond.bottom.y), diamond.bottom);
            square_BR = new Square(new Index(diamond.bottom.x,diamond.right.y), diamond.right,
                                diamond.bottom, new Index(diamond.right.x,diamond.bottom.y));
            if (diamond.top.y >= 0)
            {
                if (diamond.left.x >= 0)
                {
                    squareList.Add(square_TL);
                }
                if (diamond.right.x <= size - 1)
                {
                    squareList.Add(square_TR);
                }
            }
            if (diamond.bottom.y <= size - 1)
            {
                if (diamond.left.x >= 0)
                {
                    squareList.Add(square_BL);
                }
                if (diamond.right.x <= size - 1)
                {
                    squareList.Add(square_BR);
                }
            }
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
    }

    //bool JudgeInSide(Diamond Coord)
    //{

    //    if (Coord.center.y == 0)
    //    {
    //        return false;
    //    }
        
    //    else if (Coord.center.y == size - 1)
    //    {
    //        return false;
    //    }
        
    //    else if(Coord.center.x == 0)
    //    {
    //        return false;
    //    }
    //    else if (Coord.center.x == size - 1)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    void SetMaxHeightValue(float height)
    {
        mMaxHeightValue = (mMaxHeightValue < height) ? Mathf.RoundToInt(height) : mMaxHeightValue;
    }

    void SetMinHeightvalue(float height)
    {
        mMinHeightvalue = (mMinHeightvalue > height) ? Mathf.RoundToInt(height) : mMinHeightvalue;
    }

    GameObject[,] ShowObj;

    void ShowOnScene()
    {
        if(ShowObj == null)
            ShowObj = new GameObject[size, size];
        Debug.Log(mMaxHeightValue);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int val = Mathf.RoundToInt((float)(map[i, j] - mMinHeightvalue) / (float)(mMaxHeightValue - mMinHeightvalue) * 10);
                Color valG = Color.white * ((float)val / 10);
                if (val == 7)
                    valG = Color.black;
                if (val <= 5 && val >= 2) //Dust
                    valG = Color.yellow * 0.3f;
                if (val == 1)             // Lava
                    valG = Color.red;
                if (val == 8 || val == 6)            //Rock
                    valG = Color.gray;
                if (val == 9)
                { //Gold
                    valG = Color.yellow;
                }
                if (val == 10 || val == 0)
                { //Diamond
                    valG = new Color(0, 0.8f, 0.8f, 1);
                }
                GameObject test;
                if (ShowObj[i, j] == null)
                {
                    test = Instantiate(Resources.Load("Quad") as GameObject);
                    ShowObj[i, j] = test;
                }
                else
                    test = ShowObj[i, j];
                test.transform.position = new Vector3(i, j, 0) + transform.position - new Vector3((size - 1)/2,0,0);
                test.GetComponent<MeshRenderer>().material.color = valG;
            }
        }
    }

	// Use this for initialization
	void Start () {
        GenMap();
	}


    [ContextMenu("GenMap")]
    void GenMap()
    {
        flag = true;
        InitGenerator();
        CoreCalculate();
        Debug.Log("Done");
        ShowOnScene();
        flag = false;
    }

    bool flag = false;
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKey(KeyCode.Space) && !flag)
        //{
        //    GenMap();
        //}
	}

    public void DeleteIndex(int x, int y)
    {
        map[x, y] = -1;
        Destroy(ShowObj[x,y]);
    }

    public bool IsTileDust(Index pos)
    {
        if(map[pos.x,pos.y] == Tables.TerrainTable["Dust"].rate)
        {
            return true;
        }
        return false;
    }
    public bool IsTileRock(Index pos)
    {
        if (map[pos.x, pos.y] == Tables.TerrainTable["Rock"].rate)
        {
            return true;
        }
        return false;
    }
    public bool IsTileLava(Index pos)
    {
        if (map[pos.x, pos.y] == Tables.TerrainTable["Lava"].rate)
        {
            return true;
        }
        return false;
    }
    public bool IsTileEmpty(Index pos)
    {
        if (map[pos.x, pos.y] == Tables.TerrainTable["Empty"].rate)
        {
            return true;
        }
        return false;
    }
}
