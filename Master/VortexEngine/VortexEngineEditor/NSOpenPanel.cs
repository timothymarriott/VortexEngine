using System.Runtime.InteropServices;

namespace VortexEngine.Editor;

class NSOpenPanel
{
    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr objc_getClass(string className);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
    private static extern IntPtr sel_registerName(string selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend_IntPtr_bool(IntPtr receiver, IntPtr selector, bool arg);

    public static string ShowOpenPanel()
    {
        IntPtr nsOpenPanelClass = objc_getClass("NSOpenPanel");
        IntPtr openPanelSelector = sel_registerName("openPanel");
        IntPtr openPanelInstance = objc_msgSend(nsOpenPanelClass, openPanelSelector);

        IntPtr allowsMultipleSelectionSelector = sel_registerName("setAllowsMultipleSelection:");
        objc_msgSend_bool(openPanelInstance, allowsMultipleSelectionSelector, false);

        IntPtr canChooseDirectoriesSelector = sel_registerName("setCanChooseDirectories:");
        objc_msgSend_bool(openPanelInstance, canChooseDirectoriesSelector, true);

        IntPtr canChooseFilesSelector = sel_registerName("setCanChooseFiles:");
        objc_msgSend_bool(openPanelInstance, canChooseFilesSelector, false);

        IntPtr runModalSelector = sel_registerName("runModal");
        objc_msgSend(openPanelInstance, runModalSelector);

        IntPtr urlSelector = sel_registerName("URL");
        IntPtr url = objc_msgSend_IntPtr(openPanelInstance, urlSelector);

        if (url == IntPtr.Zero)
        {
            Console.WriteLine("No directory selected.");
            return null;
        }

        IntPtr pathSelector = sel_registerName("path");
        IntPtr pathNSString = objc_msgSend_IntPtr(url, pathSelector);

        return NSStringToString(pathNSString);
    }

    private static string NSStringToString(IntPtr nsString)
    {
        if (nsString == IntPtr.Zero) return null;

        IntPtr utf8Selector = sel_registerName("UTF8String");
        IntPtr utf8String = objc_msgSend_IntPtr(nsString, utf8Selector);

        return Marshal.PtrToStringUTF8(utf8String);
    }
}