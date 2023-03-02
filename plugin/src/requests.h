DEFINE_REQUEST("Sends a transform to mc_rtc server, the provided transform is converted from Unity world to mc_rtc",
               SendTransformRequest,
               McRtc::PTransform,
               transform_requests_,
               public)

DEFINE_REQUEST("Change a checkbox state", SendCheckboxRequest, bool, checkbox_requests_, public)

DEFINE_REQUEST("Sends an array to mc_rtc server",
               SendArrayInputRequest,
               McRtc::FloatArray,
               array_input_requests_,
               protected)