public class EnemyStats
{
    protected string _type;
    protected int _HP, _SPD, _AGI, _STR, _AP;

    public string EnemyType
    {
        get { return _type; }
        set { _type = value; }
    }

    public int HP
    {
        get { return _HP; }
        set { _HP = value; }
    }

    public int SPD
    {
        get { return _SPD; }
        set { _SPD = value; }
    }

    public int AGI
    {
        get { return _AGI; }
        set { _AGI = value; }
    }

    public int STR
    {
        get { return _STR; }
        set { _STR = value; }
    }

    public int AP
    {
        get { return _AP; }
        set { _AP = value; }
    }


    public EnemyStats(string type, int HP, int SPD, int AGI, int STR, int AP)
    {
        _type = type;
        _HP = HP;
        _SPD = SPD;
        _AGI = AGI;
        _STR = STR;
        _AP = AP;
    }
}



