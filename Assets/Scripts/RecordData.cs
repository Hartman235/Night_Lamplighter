[System.Serializable]
public class RecordData
{
    public int score;
    public string date;
    
    public RecordData(int score)
    {
        this.score = score;
        this.date = System.DateTime.Now.ToString("dd.MM.yyyy");
    }
}