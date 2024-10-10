#include <cstdlib>
#include <filesystem>
#include <string>
#include <vector>
#include <windows.h>

void panic_then_pause(const std::string &msg)
{
    std::string command = "cmd.exe /c echo " + msg + " && pause";
    system(command.c_str());
    exit(1);
}

int main(int argc, char *argv[])
{
    std::vector<std::string> args(argv, argv + argc);

    // 获取当前工作目录
    std::filesystem::path working_path = std::filesystem::current_path();
    std::filesystem::path root_directory = working_path / "hollow_app";

    // 检查 hollow_app 目录是否存在
    if (!std::filesystem::exists(root_directory))
    {
        panic_then_pause(root_directory.string() + " does not exist");
    }
    else if (!std::filesystem::is_directory(root_directory))
    {
        panic_then_pause(root_directory.string() + " is not a directory");
    }

    // 查找可执行文件
    std::filesystem::path executable = root_directory / "Hollow.Windows.exe";
    if (!std::filesystem::exists(executable))
    {
        panic_then_pause(executable.string() + " not found");
    }

    // 设置执行命令
    STARTUPINFO si = {sizeof(si)};
    PROCESS_INFORMATION pi;
    std::string command = executable.string();

    if (argc > 1)
    {
        for (int i = 1; i < argc; ++i)
        {
            command += " " + args[i];
        }
    }

    if (!CreateProcess(NULL, const_cast<char *>(command.c_str()), NULL, NULL, FALSE, 0, NULL,
                       root_directory.string().c_str(), &si, &pi))
    {
        panic_then_pause("Failed to start process");
    }

    // 关闭进程句柄
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    return 0;
}
