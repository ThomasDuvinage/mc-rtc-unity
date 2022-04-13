#include <mc_control/ControllerClient.h>

#include <mc_rbdyn/RobotLoader.h>

#include <mc_rtc/config.h>

#include <SpaceVecAlg/Conversions.h>

#include <boost/filesystem.hpp>
namespace bfs = boost::filesystem;

#ifdef MC_RTC_HAS_ROS_SUPPORT
#  include <ros/package.h>
#endif

using on_robot_callback_t = void (*)(const char * id);
on_robot_callback_t on_robot_callback = nullptr;

using on_robot_mesh_callback_t = void (*)(const char * id,
                                          const char * name,
                                          const char * path,
                                          float scale,
                                          float qw,
                                          float qx,
                                          float qy,
                                          float qz,
                                          float tx,
                                          float ty,
                                          float tz);
on_robot_mesh_callback_t on_robot_mesh_callback = nullptr;

using on_remove_robot_callback_t = void (*)(const char * id);
on_remove_robot_callback_t on_remove_robot_callback = nullptr;

inline mc_rbdyn::RobotModulePtr fromParams(const std::vector<std::string> & p)
{
  mc_rbdyn::RobotModulePtr rm{nullptr};
  if(p.size() == 1)
  {
    rm = mc_rbdyn::RobotLoader::get_robot_module(p[0]);
  }
  if(p.size() == 2)
  {
    rm = mc_rbdyn::RobotLoader::get_robot_module(p[0], p[1]);
  }
  if(p.size() == 3)
  {
    rm = mc_rbdyn::RobotLoader::get_robot_module(p[0], p[1], p[2]);
  }
  if(p.size() > 3)
  {
    mc_rtc::log::warning("Too many parameters provided to load the robot, complain to the developpers of this package");
  }
  return rm;
}

inline bfs::path convertURI(const std::string & uri, [[maybe_unused]] std::string_view default_dir = "")
{
  const std::string package = "package://";
  if(uri.size() >= package.size() && uri.find(package) == 0)
  {
    size_t split = uri.find('/', package.size());
    std::string pkg = uri.substr(package.size(), split - package.size());
    auto leaf = bfs::path(uri.substr(split + 1));
    bfs::path MC_ENV_DESCRIPTION_PATH(mc_rtc::MC_ENV_DESCRIPTION_PATH);
#ifndef __EMSCRIPTEN__
//#  ifndef MC_RTC_HAS_ROS_SUPPORT
#  if 1
    // FIXME Prompt the user for unknown packages
    if(pkg == "jvrc_description")
    {
      pkg = (MC_ENV_DESCRIPTION_PATH / ".." / "jvrc_description").string();
    }
    else if(pkg == "mc_env_description")
    {
      pkg = MC_ENV_DESCRIPTION_PATH.string();
    }
    else if(pkg == "mc_int_obj_description")
    {
      pkg = (MC_ENV_DESCRIPTION_PATH / ".." / "mc_int_obj_description").string();
    }
    else
    {
      pkg = default_dir;
    }
#  else
    //  FIXME Unity Hub and Unity Editor don't have the environment variables we set in the shell for ROS
    pkg = ros::package::getPath(pkg);
#  endif
#else
    pkg = "/assets/" + pkg;
#endif
    return pkg / leaf;
  }
  const std::string file = "file://";
  if(uri.size() >= file.size() && uri.find(file) == 0)
  {
    return bfs::path(uri.substr(file.size()));
  }
  return uri;
}

struct UnityClient : public mc_control::ControllerClient
{
  using mc_control::ControllerClient::ControllerClient;
  using ElementId = mc_control::ElementId;

  void started() override
  {
    for(auto & it : robots_seen)
    {
      it.second = false;
    }
  }

  void category(const std::vector<std::string> & parent, const std::string & category) override {}

  void robot(const ElementId & id,
             const std::vector<std::string> & parameters,
             const std::vector<std::vector<double>> & q,
             const sva::PTransformd & posW) override
  {
    if(!on_robot_callback)
    {
      return;
    }
    std::string rid;
    for(const auto & cat : id.category)
    {
      rid += cat;
      rid += "\n";
    }
    rid += id.name;
    robots_seen[rid] = true;
    if(!robots.count(rid) || modules[rid]->parameters() != parameters)
    {
      modules[rid] = fromParams(parameters);
      if(modules[rid])
      {
        robots[rid] = mc_rbdyn::loadRobot(*modules[rid]);
      }
    }
    auto robots_ptr = robots[rid];
    if(!robots_ptr)
    {
      return;
    }
    auto & robot = robots_ptr->robot();
    robot.mbc().q = q;
    robot.posW(posW);
    if(on_robot_callback)
    {
      on_robot_callback(rid.c_str());
    }
    const auto & bodies = robot.mb().bodies();
    const auto & robot_visuals = robot.module()._visual;
    for(size_t bIdx = 0; bIdx < bodies.size(); ++bIdx)
    {
      const auto & b = bodies[bIdx];
      if(!robot_visuals.count(b.name()))
      {
        continue;
      }
      size_t i = 0;
      const auto & visuals = robot_visuals.at(b.name());
      for(const auto & v : visuals)
      {
        auto X_0_visual = v.origin * robot.mbc().bodyPosW[bIdx];
        std::string name = b.name();
        if(visuals.size() > 1)
        {
          name = fmt::format("{}_{}", name, ++i);
        }
        using Geometry = rbd::parsers::Geometry;
        switch(v.geometry.type)
        {
          case Geometry::MESH:
            mesh_callback(rid, name, robot.module().path, v, X_0_visual);
            break;
          default:
            break;
        }
      }
    }
  }

  void mesh_callback(const std::string & rid,
                     const std::string & name,
                     const std::string & mesh_path,
                     const rbd::parsers::Visual & visual,
                     const sva::PTransformd & X_0_visual)
  {
    static auto to_unity = []() -> Eigen::Matrix4f {
      Eigen::Matrix4f out = Eigen::Matrix4f::Zero();
      out(0, 0) = 1.0f;
      out(1, 2) = 1.0f;
      out(2, 1) = 1.0f;
      out(3, 3) = 1.0f;
      return out;
    }();
    auto homo =
        to_unity * sva::conversions::toHomogeneous(X_0_visual.cast<float>(), sva::conversions::RightHanded) * to_unity;
    Eigen::Quaternionf q(homo.block<3, 3>(0, 0));
    const auto & mesh = boost::get<rbd::parsers::Geometry::Mesh>(visual.geometry.data);
    Eigen::Vector3f t = homo.block<3, 1>(0, 3);
    std::string path = convertURI(mesh.filename, mesh_path).string();
    on_robot_mesh_callback(rid.c_str(), name.c_str(), path.c_str(), static_cast<float>(mesh.scale), q.w(), q.x(),
                           q.y(), q.z(), t.x(), t.y(), t.z());
  }

  void stopped() override
  {
    for(const auto & it : robots_seen)
    {
      if(!it.second && on_remove_robot_callback)
      {
        on_remove_robot_callback(it.first.c_str());
      }
    }
  }

  void start()
  {
    mc_control::ControllerClient::start();
  }

  std::map<std::string, mc_rbdyn::RobotModulePtr> modules;
  std::map<std::string, std::shared_ptr<mc_rbdyn::Robots>> robots;
  std::map<std::string, bool> robots_seen;
};

static std::unique_ptr<UnityClient> client;

extern "C"
{
  void StartClient()
  {
    client = std::make_unique<UnityClient>("ipc:///tmp/mc_rtc_pub.ipc", "ipc:///tmp/mc_rtc_rep.ipc", 1.0);
    client->start();
  }

  void RegisterCallbacks(on_robot_callback_t robot_callback,
                         on_robot_mesh_callback_t robot_mesh_callback,
                         on_remove_robot_callback_t remove_robot_callback)
  {
    on_robot_callback = robot_callback;
    on_robot_mesh_callback = robot_mesh_callback;
    on_remove_robot_callback = remove_robot_callback;
  }

  void StopClient()
  {
    if(client && on_remove_robot_callback)
    {
      for(const auto & it : client->robots)
      {
        on_remove_robot_callback(it.first.c_str());
      }
    }
    client.reset(nullptr);
    // FIXME We probably want a mutex here to avoid resetting the callback when it should still exist
    on_robot_callback = nullptr;
    on_robot_mesh_callback = nullptr;
    on_remove_robot_callback = nullptr;
  }
}
