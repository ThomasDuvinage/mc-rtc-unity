DEFINE_REQUEST("Sends a transform to mc_rtc server, the provided transform is converted from Unity world to mc_rtc",
               SendTransformRequest,
               McRtc::PTransform,
               transform_requests_,
               McRtc::FromUnity)
