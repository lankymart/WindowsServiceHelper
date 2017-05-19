# Windows Service Helper

Original project by [andersonimes](https://github.com/andersonimes) this is just a clone taken from [CodePlex](https://windowsservicehelper.codeplex.com/).
All attempts have been made to provide the correct attribution and licensing.

---

## What is Service Helper

Being someone who writes Windows Services a lot, it can be frustrating to deal with the headaches involved in debugging services. Often it involves tricks, hacks, and partial workarounds to test all of your code. There is no "just hit F5" experience for Windows Services developers.

Service Helper solves this by triggering a UI to be shown if a debugger is attached that simulates (as closely as possible) the Windows Services Environment.

![Image of GUI](http://download-codeplex.sec.s-msft.com/Download?ProjectName=windowsservicehelper&DownloadId=252491)

Just hit F5, and this UI automatically appears. If there is no debugger attached, your service will execute as normal and can be installed in the Windows Services system.

## How to use?

The easiest way to get Windows Service Helper in your project is to use the NuGet package ServiceProcess.Helpers on the NuGet official feed.

Simply make a few changes to the typical code in the "Program.cs" for your application:
using System.ServiceProcess;
using ServiceProcess.Helpers; //HERE

```
namespace DemoService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] 
			{ 
				new Service1() 
			};

            //ServiceBase.Run(ServicesToRun);
            ServicesToRun.LoadServices(); //AND HERE
        }
    }
}
```

That's it!

## Future Enhancements

* NuGet package for easy inclusion in your project (Available now as [ServiceProcess.Helpers](http://nuget.org/List/Packages/ServiceProcess.Helpers))
* Less ugly UI
* More closely simulating the Windows Services environment and allowing calls like "RequestAdditionalTime" and enforcing timeouts.
*.NET Framework 3.5 Support
*Autostart preferences remembered across debugging sessions

## Fork This Project!

This code is not perfection. If you have a bugfix or enhancement you'd like to see in Windows Service Helper, JUST FORK IT!. Send me a pull request and I will put your name in lights (er... maybe here on this wiki page at least).
