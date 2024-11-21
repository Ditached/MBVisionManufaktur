using System;
using UnityEngine;
using UnityEngine.Playables;

public class IntroController : MonoBehaviour
{
    public PlayableDirector introTimeline;
    
    private AppState lastState = AppState.Waiting;

    private void Update()
    {
        if (lastState != UpdatePackage.globalAppState)
        {
            lastState = UpdatePackage.globalAppState;
            
            switch (lastState)
            {
                case AppState.Waiting:
                    introTimeline.time = 0;
                    introTimeline.Evaluate();
                    introTimeline.Stop();
                    break;
                case AppState.Running:
                    introTimeline.time = 0;
                    introTimeline.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
