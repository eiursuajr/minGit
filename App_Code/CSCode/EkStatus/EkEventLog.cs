using System;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Collections;

/// <summary>
/// Summary description for EkEventLog
/// </summary>
public class EkEventLog
{
    public const string SPLIT_VAL = "!@#$%";

	public EkEventLog()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public ArrayList GetApplicationEntries(int nNumEntries)
    {
        return GetEventLogEntries("Application", nNumEntries);
    }

    public ArrayList GetSystemEntries(int nNumEntries)
    {
        return GetEventLogEntries("System", nNumEntries);
    }
    
    public ArrayList GetSecurityEntries(int nNumEntries)
    {
        return GetEventLogEntries("Security", nNumEntries);
    }
    
    public ArrayList GetCustomEntries(string slogName, int nNumEntries)
    {
        return GetEventLogEntries(slogName, nNumEntries);
    }

    private ArrayList GetEventLogEntries(string slogName, int nNumEntries)
    {
        ArrayList saReturn = new ArrayList();
        EventLog eventLog = new EventLog(slogName);
        EventLogEntryCollection saEvents;
        int nCount = 0;

        saEvents = eventLog.Entries;

        if (nNumEntries > eventLog.Entries.Count)
            nCount = 0;
        else
            nCount = eventLog.Entries.Count - nNumEntries;

        for (int i = eventLog.Entries.Count - 1; i >= nCount; i--)
            saReturn.Add(saEvents[i].Source + SPLIT_VAL + saEvents[i].EntryType + SPLIT_VAL + saEvents[i].Message + SPLIT_VAL + saEvents[i].TimeGenerated.ToString());

        //----- Cleanup
        if (eventLog != null)
            eventLog = null;

        return saReturn;
    }
}
