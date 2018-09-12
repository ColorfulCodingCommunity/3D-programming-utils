///SOURCE: https://github.com/rizasif/vuforia-runtime-dataset-sample/blob/master/Assets/DatasetLoader.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaGenerateTrackersFromDataset : MonoBehaviour {

    public string datasetName;
    public GameObject prefab;

	void Start ()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
    }
	
	void OnVuforiaStarted()
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        objectTracker.Stop();
        IEnumerable<DataSet> dataSetList = objectTracker.GetActiveDataSets();
        foreach(DataSet set in dataSetList)
        {
            if(set.Path != ("Vuforia/" + datasetName + ".xml"))
            {
                objectTracker.DeactivateDataSet(set);
                continue;
            }
            objectTracker.Start();

            IEnumerable<TrackableBehaviour> trackableBehaviours = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in trackableBehaviours)
            {
                if (set.Contains(tb.Trackable))
                {

                    GameObject go = tb.gameObject;
                    go.AddComponent<DefaultTrackableEventHandler>();
                    //Insert code here : 
                    GameObject model = Instantiate(prefab) as GameObject;
                    model.transform.parent = tb.transform;
                    model.SetActive(true);

                    tb.gameObject.SetActive(true);
                }
            }
            break;
        }
    }
}
