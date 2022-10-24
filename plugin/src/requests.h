DEFINE_REQUEST("Sends a transform to mc_rtc server, the provided transform is converted from Unity world to mc_rtc",
               SendTransformRequest,
               McRtc::PTransform,
               transform_requests_,
               McRtc::FromUnity)

DEFINE_VOID_REQUEST("Change a checkbox state", SendCheckboxRequest, checkbox_requests_)
