using UnityEngine;

public class Mission
{
    private int     missionId;
    private string  missionKey;
    private string  missionText;
    private Transform marker;
    private bool isRequireSilence;

    public int Id                   { get { return missionId; }     set { missionId     = value; } }
    public string Text              { get { return missionText; }   set { missionText   = value; } }
    public string Key               { get { return missionKey; }    set { missionKey    = value; } }
    public Transform Marker         { get { return marker; }        set { marker        = value; } }
    public bool IsRequireSilence    { get { return marker; }}

    public Mission(int missionId, string missionKey, string missionText, Transform marker = null, bool isRequireSilence = false)
    {
        this.missionId      = missionId;
        this.missionText    = missionText;
        this.missionKey     = missionKey;

        // for optional args
        this.marker             = marker;
        this.isRequireSilence   = isRequireSilence;
    }

}
