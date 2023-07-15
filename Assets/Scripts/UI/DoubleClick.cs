using UnityEngine;
using System.Timers;

public class DoubleClick : MonoBehaviour
{
    private readonly Timer _MouseSingleClickTimer = new Timer();

    // Start is called before the first frame update.
    void Start()
    {
        _MouseSingleClickTimer.Interval = 400;
        _MouseSingleClickTimer.Elapsed += SingleClick;
    }

    void SingleClick(object o, System.EventArgs e)
    {
        _MouseSingleClickTimer.Stop();

        Debug.Log("Single Click");
        //Do your stuff for single click here....
    }
    public void DoubleClck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_MouseSingleClickTimer.Enabled == false)
            {
                // ... timer start
                _MouseSingleClickTimer.Start();
                // ... wait for double click...
                return;
            }
            else
            {
                //Doubleclick performed - Cancel single click
                _MouseSingleClickTimer.Stop();

                //Do your stuff here for double click...
                Debug.Log("Double Click");
                
            }
        }
    }

    private void Update()
    {
        DoubleClck();
    }

}