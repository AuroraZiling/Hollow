#include <cstdlib>
#include <filesystem>
#include <iostream>
#include <string>
#include <vector>
#include <windows.h>

namespace fs = std::filesystem;

void panic_then_pause(const std::string &msg)
{
    std::cerr << msg << std::endl;
    system("pause");
    exit(1);
}

int main(int argc, char *argv[])
{
    // Get Hollow App Directory
    fs::path working_path = fs::current_path();
    fs::path root_directory = working_path / "hollow_app";

    if (!fs::exists(root_directory))
    {
        panic_then_pause(root_directory.string() + " does not exist");
    }
    else if (!fs::is_directory(root_directory))
    {
        panic_then_pause(root_directory.string() + " is not a directory");
    }

    fs::path executable = root_directory / "Hollow.Windows.exe";
    if (!fs::exists(executable))
    {
        panic_then_pause(executable.string() + " not found");
    }

    // Prepare command line to execute
    std::vector<std::string> args;
    for (int i = 1; i < argc; ++i)
    {
        args.push_back(argv[i]);
    }

    // Construct command
    std::string command = "\"" + executable.string() + "\"";
    for (const auto &arg : args)
    {
        command += " \"" + arg + "\"";
    }

    // Change directory and run the command
    std::string change_dir_command = "cd /d " + root_directory.string() + " && " + command;
    system(change_dir_command.c_str());

    return 0;
}