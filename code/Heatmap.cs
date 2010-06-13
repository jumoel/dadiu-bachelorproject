using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class Heatmap : MonoBehaviour
{

    public bool record;
    public List<Vector4> heatOutput = new List<Vector4>();
	public GameObject container;
	public String logName;
    private Vector4 heatGather;
    public Transform target;
    private float theTimeBefore;
    private float theTime;
    private int numOfLogFiles = 0;
    private string fileName;
    private bool hasWritten;
    private float minX;
    private float minY;
    private float minZ;
    private float maxX;
    private float maxY;
    private float maxZ;

    // Use this for initialization
    void Start()
    {
        fileName = Application.loadedLevelName + "_" + transform.name.ToString() + "_log_" + numOfLogFiles.ToString() + ".log";
		if (logName !=null && logName != "") {
			fileName = logName + "_" + fileName;
		}

		if (container == null) {
			CalculateSize();
		} else {
			if (container.transform.renderer != null) {
				Bounds bounds = container.transform.renderer.bounds;
				
				minX = (bounds.center - bounds.extents).x;
				minY = (bounds.center - bounds.extents).y;
				minZ = (bounds.center - bounds.extents).z;
				maxX = (bounds.center + bounds.extents).x;
				maxY = (bounds.center + bounds.extents).y;
				maxZ = (bounds.center + bounds.extents).z;
			}
		}

    }


    void CalculateSize()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (GameObject g in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (g.transform.renderer != null)
            {
                bounds.Encapsulate(g.transform.renderer.bounds);
            }
        }

        minX = (bounds.center - bounds.extents).x;
        minY = (bounds.center - bounds.extents).y;
        minZ = (bounds.center - bounds.extents).z;
        maxX = (bounds.center + bounds.extents).x;
        maxY = (bounds.center + bounds.extents).y;
        maxZ = (bounds.center + bounds.extents).z;
    }



    public void writeListToFile(string destFile)
    {
        //always use a try...catch to deal 
        //with any exceptions that may occur
        try
        {

            //check if the destination file exists,
            //if it does, we generate a new name,
            //will raise an exception otherwise
            while (System.IO.File.Exists(destFile))
            {
                numOfLogFiles++;
                destFile = transform.name.ToString() + "_log_" + numOfLogFiles.ToString() + ".log";
            }

            // Values to be inserted before the actual heatmap data.
            int extraValues = 7;

            Vector4[] outputArray = heatOutput.ToArray();
            int outArrayLength = outputArray.Length + extraValues;
            object[] obj = new object[outArrayLength];

            obj[0] = "Min X value: " + minX.ToString();
            obj[1] = "Min Y value: " + minY.ToString();
            obj[2] = "Min Z value: " + minZ.ToString();
            obj[3] = "Max X value: " + maxX.ToString();
            obj[4] = "Max Y value: " + maxY.ToString();
            obj[5] = "Max Z value: " + maxZ.ToString();
            obj[6] = "Key: charpos in X , Y , Z , Time since start capture";


            int j = extraValues;
            for (int i = 0; i < outputArray.Length; i++)
            {
                obj[j] = (outputArray[i].x.ToString() + " , " + outputArray[i].y.ToString() + " , " + outputArray[i].z.ToString() + " , " + outputArray[i].w.ToString());
                j++;
            }

            // use ,true if you want to append data to file
            // this process will open a save file dialog and give the option to choose
            // file location, name, and ext.  then when you press save it will save it

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(destFile))
            {
                foreach (string line in obj)
                {
                    file.WriteLine(line);
                }
            }
            Debug.Log("File written successfully");
        }
        catch (Exception ex)
        {
            //handle any errors that occurred
            Debug.Log(ex.Message);
        }
    }

    public float starttid()
    {
        record = true;
        hasWritten = false;
        Debug.Log("The time before " + theTimeBefore);
        return theTimeBefore;
    }
    public float sluttid()
    {
        record = false;
        writeListToFile(fileName);
        hasWritten = true;

        // Reset the list
        heatOutput = new List<Vector4>();
        Debug.Log("The time " + theTime);
        return theTime;
    }

    public void recordEvent(string eventName)
    {
        int numOfSameEventFiles = 0;
        eventName = transform.name.ToString() + "_log_" + numOfSameEventFiles.ToString() + ".log";

        try
        {
            while (System.IO.File.Exists(eventName))
            {
                numOfSameEventFiles++;
                eventName = transform.name.ToString() + "_log_" + numOfSameEventFiles.ToString() + ".log";
            }

            string eventLine = heatOutput[(heatOutput.Count - 1)].ToString();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(eventName))
                file.WriteLine(eventLine);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (record)
        {
            // Remove the next line when the functions are called the way they are supposed to be.
            if (hasWritten)
                starttid();
            theTime = Time.time - theTimeBefore;
            heatGather = new Vector4(target.transform.position.x, target.transform.position.y, target.transform.position.z, theTime);
            heatOutput.Add(heatGather);
        }
        else
        {
            if (!hasWritten)
                sluttid();

            theTimeBefore = Time.time;
        }
    }
}
