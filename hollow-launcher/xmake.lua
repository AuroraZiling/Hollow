add_rules("mode.release", "mode.debug")
add_rules("plugin.compile_commands.autoupdate", {outputdir = "./"})

target("HollowLauncher")
    set_kind("binary")
    set_languages("cxx20")
    add_files("src/main.cpp")
    add_files("HollowLauncher.rc")
    if is_os("windows") then 
        add_cxflags("/utf-8")
        add_rules("win.sdk.application")
    end
target_end()
