public class EnemyStats
{
    protected string _type;
    protected int _HP, _SPD, _AGI;

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

    public EnemyStats(string type, int HP, int SPD, int AGI)
    {
        _type = type;
        _HP = HP;
        _SPD = SPD;
        _AGI = AGI;
    }
}



