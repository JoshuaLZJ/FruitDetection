using System;

public struct ImageSettings
{
    //public const int imageHeight = 640;
    //public const int imageWidth = 640;
    public const int imageHeight = 512;
    public const int imageWidth = 512;
}

// For checking tensor names, you can open the TF model .pb file with tools like Netron: https://github.com/lutzroeder/netron
public struct TensorFlowModelSettings
{
    // input tensor name
    //public const string inputTensorName = "serving_default_input_tensor:0";
    public const string inputTensorName = "input_tensor:0";

    // output tensors name
    public const string detection_boxes = "detection_boxes";
    public const string detection_scores = "detection_scores";
    public const string detection_classes = "detection_classes";
}