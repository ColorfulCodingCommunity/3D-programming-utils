// OpenCV_Bridge_Windows_Dll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include <opencv2/opencv.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <iostream>
#include <stdio.h>
#include <fstream>

using namespace std;

struct KeypointsInfo {
	int ptx;
	int pty;
	int octave;
	float angle;
	float response;
};

extern "C" float __declspec(dllexport) __stdcall Foopluginmethod() {
	cv::Mat img(10, 10, CV_8UC1);
	return img.rows * 1.0f;
}

extern "C" void __declspec(dllexport) __stdcall GetRawImage(unsigned char * data, int width, int height) {
	cv::Mat texture(height, width, CV_8UC4, data);
	cv::cvtColor(texture, texture, cv::COLOR_BGRA2GRAY);
	cv::cvtColor(texture, texture, cv::COLOR_GRAY2RGBA);
	std::memcpy(data, texture.data, texture.total() * texture.elemSize());
}

extern "C" bool __declspec(dllexport) __stdcall GetFeaturePointsPosition(unsigned char * data, int width, int height, KeypointsInfo** keypoints, int *length, unsigned char** descriptors, int* descLength) {
	cv::Mat texture(height, width, CV_8UC4, data);
	cv::cvtColor(texture, texture, cv::COLOR_BGRA2GRAY);

	cv::Ptr<cv::FeatureDetector> detector = cv::ORB::create();
	cv::Ptr<cv::DescriptorExtractor> extractor = cv::ORB::create();

	vector<cv::KeyPoint> kp;
	cv::Mat desc;
	detector->detectAndCompute(texture, cv::noArray() , kp, desc);

	vector<KeypointsInfo> kps;
	//Return keypoints in custom structure
	for (vector<cv::KeyPoint>::iterator it = kp.begin(); it != kp.end(); ++it) {
		KeypointsInfo kpInfo;
		
		kpInfo.ptx = it->pt.x;
		kpInfo.pty = it->pt.y;
		kpInfo.angle = it->angle;
		kpInfo.octave = it->octave;
		kpInfo.response = it->response;

		kps.push_back(kpInfo);
	}

	*length = kps.size();
	*keypoints = &kps[0];

	//Return descriptors
	vector<unsigned char> descArray;
	for (int i = 0; i < desc.rows; ++i) {
		descArray.insert(descArray.end(), desc.ptr<uchar>(i), desc.ptr<uchar>(i) + desc.cols);
	}
	*descLength = desc.rows;

	unsigned char* myArr = &descArray[0];
	*descriptors = myArr;
	
	//Add KPs on texture
	cv::Mat img_keypoints;

	cv::drawKeypoints(texture, kp, img_keypoints);
	cv::cvtColor(img_keypoints, img_keypoints, cv::COLOR_RGB2RGBA);

	std::memcpy(data, img_keypoints.data, img_keypoints.total() * img_keypoints.elemSize());

	std::ofstream log_file(
		"log_file.txt", std::ios_base::out | std::ios_base::app);
	
	for (int i = 0; i < *descLength * 32; i++) {

		log_file << (int)*(myArr + i) << " ";
	}
	log_file.close();

	return true;
}


extern "C" void __declspec(dllexport) __stdcall ReleaseMemory(int* pArray) {
	delete[] pArray;

	return;
}