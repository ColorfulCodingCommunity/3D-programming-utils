#include "OpenCV_Bridge.h"
#include <opencv2/core.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/features2d.hpp>

#include <vector>


#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "OpenCV_Bridge", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "OpenCV_Bridge", __VA_ARGS__))

using namespace std;

extern "C" {
	float OpenCV_Bridge::Foopluginmethod() {
		cv::Mat img(10, 10, CV_8UC1);
		return img.rows * 1.0f;
	}

	void OpenCV_Bridge::GetRawImage(unsigned char * data, int width, int height){
		cv::Mat texture(height, width, CV_8UC4, data);
		cv::cvtColor(texture, texture, cv::COLOR_BGRA2GRAY);
		cv::cvtColor(texture, texture, cv::COLOR_GRAY2RGBA);
		std::memcpy(data, texture.data, texture.total() * texture.elemSize());
	}


	bool OpenCV_Bridge::GetFeaturePointsPosition(unsigned char * data, int width, int height, KeypointsInfo** keypoints, int *length, unsigned char** descriptors, int* descLength) {
		cv::Mat texture(height, width, CV_8UC4, data);
		cv::cvtColor(texture, texture, cv::COLOR_BGRA2GRAY);

		cv::Ptr<cv::FeatureDetector> detector = cv::ORB::create();
		cv::Ptr<cv::DescriptorExtractor> extractor = cv::ORB::create();

		vector<cv::KeyPoint> kp;
		cv::Mat desc;

		detector->detectAndCompute(texture, cv::noArray(), kp, desc);

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

		KeypointsInfo* kpsArray = &kps[0];

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

		return true;
	}

	void OpenCV_Bridge::ReleaseMemory(int* pArray) {
		delete[] pArray;

		return;
	}
}
