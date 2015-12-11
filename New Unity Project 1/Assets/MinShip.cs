using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinShip : MonoBehaviour {
    public enum MapDir
    {
        Up,
        Down,
        Left,
        Right,
        None
    };

    int hp;

    int view;

    int speed;

    bool isFall;

    public static Dictionary<MapDir, Index> DirTable = new Dictionary<MapDir, Index>{
        { MapDir.Up , new Index(0,1) },
        { MapDir.Down , new Index(0,-1) },
        { MapDir.Left, new Index(-1,0) },
        { MapDir.Right, new Index(1,0) },
        { MapDir.None, new Index(0,0) }
    };

    Map map;

    class Detector
    {
        int detectDis;

        int identifyLeve;

        int mDetectDis;

        public Detector(int detectDis,int identifyLeve,int mDetectDis)
        {
            this.detectDis = detectDis;
            this.identifyLeve = detectDis;
            this.mDetectDis = mDetectDis;
        }

        public void ActiveD(Transform ship)
        {
            Index shipIndex = new Index();
            for (int i = 1; i < detectDis; i++)
            {
                for (int j = 0; j < i * 2 + 1; j++)
                {
                    
                }
            }
        }

        public void PassiveD(Transform ship)
        {
            ActiveD(ship);
        }

        IEnumerator Wave()
        {
            
            yield return 0; 
        }
    };

    class Drill
    {
        public int minSpeed;
        public int toolLeve;
        public bool isDigging;

        public Drill(int minSpeed,int toolLeve)
        {
            this.minSpeed = minSpeed;
            this.toolLeve = toolLeve;
            this.isDigging = false;
        }

        public void Dig(Index toDig,Map map)
        {
            map.DeleteIndex(toDig.x, toDig.y);
        }

        IEnumerator Dig(int leve)
        {
            isDigging = true;
            for (float timer = 0.0f; timer < minSpeed * (minSpeed - leve + 1); timer += Time.deltaTime)
            {
                yield return 0;
            }
            isDigging = false;
        }
    };

    Drill drill;
    Detector detector;

    public void InitShip()
    {
        map = GameObject.Find("Map").GetComponent<Map>();
        drill = new Drill(1, 1);
        detector = new Detector(3, 1, 5);
    }

    bool Moveship(MapDir dir)
    {
        if(drill.isDigging)
            return false;

        Vector3 mSpeed = Vector3.zero;
        //if (dir == MapDir.Up) //speed down
        //    mSpeed /= 2;

        //if (dir == MapDir.Down)
            //mSpeed *= 1.5f;

        Index ship = new Index(Mathf.RoundToInt(transform.position.x) + map.size / 2, 
            Mathf.RoundToInt(transform.position.y));
        Index moveTo = ship + DirTable[dir];

        if (map.IsTileEmpty(ship + new Index(0, -1)) || map.IsTileLava(ship + new Index(0, -1)))
        {
            isFall = true;
        }
        else{
            isFall = false;
        }

        if (map.IsTileDust(moveTo))
        {
            if (drill.toolLeve < Tables.TerrainTable["Dust"].leve)
                return false;

            drill.Dig(moveTo, map);
            return true;
        }
        if (map.IsTileRock(moveTo))
        {
            if (drill.toolLeve < Tables.TerrainTable["Rock"].leve)
                return false;

            drill.Dig(moveTo, map);
        }

        return true;
    }

    IEnumerator Fall(float mSpeed, MapDir dir)
    {
        Vector3 moveVector = new Vector3(DirTable[dir].x, DirTable[dir].y, 0);
        for (float timer = 0.0f; timer < 1 / mSpeed; timer += Time.deltaTime)
        {
            transform.position += moveVector * Time.deltaTime;
            yield return 0;
        }
    }

    void InputHandle()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Moveship(MapDir.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Moveship(MapDir.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Moveship(MapDir.Right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Moveship(MapDir.Up);
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            detector.ActiveD(transform);
        }
    }
        
	// Use this for initialization
	void Start () {
        InitShip();
	}

    float dTimer = 0.0f;

	// Update is called once per frame
	void Update () {
        InputHandle();
        if (dTimer >= 3.0f)
        {
            detector.PassiveD(transform);
            dTimer = 0.0f;
        }
        dTimer += Time.deltaTime;
	}
}
