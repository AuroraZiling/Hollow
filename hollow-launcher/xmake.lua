target("HollowLauncher")
    set_kind("binary")
    set_languages("cxx20")
    add_files("src/main.cpp")
    add_files("HollowLauncher.rc")

    if is_plat("windows") then
        add_defines("_WINDOWS")
    end
target_end()