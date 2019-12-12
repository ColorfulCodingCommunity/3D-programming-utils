#pragma once


#include "KeypointsInfo.h"

extern "C" {
	namespace OpenCV_Bridge
	{
		float Foopluginmethod();
		void GetRawImage(unsigned char *data, int width, int height);
		bool GetFeaturePointsPosition(unsigned char *data, int width, int height, KeypointsInfo** keypoints, int *length, unsigned char** descriptors, int* descLength);
		void ReleaseMemory(int* pArray);
	};
}

