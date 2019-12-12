using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImageVisionModule : MonoBehaviour
{
    private Color32[] texDataColor;
    private GCHandle pixelHandle;
    private IntPtr imgDataPtr;

    [HideInInspector]
    public List<KeypointsInfo> kps;
    [HideInInspector]
    public byte[] descs;
    [HideInInspector]
    public int descriptorsSize;

#if UNITY_EDITOR
    [DllImport("OpenCV_Bridge_Windows_Dll")]
    private static extern float Foopluginmethod();
    [DllImport("OpenCV_Bridge_Windows_Dll")]
    private static extern float GetRawImage(IntPtr texData, int width, int height);
    [DllImport("OpenCV_Bridge_Windows_Dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool GetFeaturePointsPosition(IntPtr texData, int width, int height, out IntPtr keypoints, out int size, out IntPtr descriptors, out int descSize);
    [DllImport("OpenCV_Bridge_Windows_Dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ReleaseMemory(IntPtr ptr);
#elif UNITY_ANDROID
    [DllImport("OpenCV_Bridge")]
    private static extern float Foopluginmethod();
    [DllImport("OpenCV_Bridge")]
    private static extern float GetRawImage(IntPtr texData, int width, int height);
    [DllImport("OpenCV_Bridge", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool GetFeaturePointsPosition(IntPtr texData, int width, int height, out IntPtr keypoints, out int size, out IntPtr descriptors, out int descSize);
    [DllImport("OpenCV_Bridge", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ReleaseMemory(IntPtr ptr);
#endif

    public Texture2D GetImageKeypoints(Texture2D texData)
    {
        texDataColor = texData.GetPixels32();

        pixelHandle = GCHandle.Alloc(texDataColor, GCHandleType.Pinned);
        imgDataPtr = pixelHandle.AddrOfPinnedObject();

        var initKepointsPtr = IntPtr.Zero;
        var initDescriptorsPtr = IntPtr.Zero;

        if (!GetFeaturePointsPosition(imgDataPtr, texData.width, texData.height, out initKepointsPtr, out int size, out initDescriptorsPtr, out descriptorsSize))
        {
            Debug.LogError("No kp returned :/");
            return null;
        }

        kps = new List<KeypointsInfo>();
        var kpsEntrySize = Marshal.SizeOf(typeof(KeypointsInfo));
        var arrayValue = initKepointsPtr;
        for(var i=0; i<size; i++)
        {
            var curr = (KeypointsInfo)Marshal.PtrToStructure(arrayValue, typeof(KeypointsInfo));
            kps.Add(curr);
            arrayValue = new IntPtr(arrayValue.ToInt32() + kpsEntrySize);
        }

        var descriptors = initDescriptorsPtr;
        descs = new byte[descriptorsSize * 32];
        Marshal.Copy(descriptors, descs, 0, descriptorsSize * 32);
        
        //TODO: Release memory!
        /*
        ReleaseMemory(initKepointsPtr);
        ReleaseMemory(initDescriptorsPtr);
        ReleaseMemory(imgDataPtr);
        */

        var newTex = new Texture2D(texData.width, texData.height, TextureFormat.RGBA32, false);

        newTex.SetPixels32(texDataColor);
        newTex.Apply();

        return newTex;
    }

    private void OnApplicationQuit()
    {
        if (pixelHandle != null && pixelHandle.IsAllocated)
        {
            pixelHandle.Free();
        }
    }
}
