#![windows_subsystem = "windows"]

use std::env;
use std::process::{Command, exit};

fn panic_then_pause(msg: String) {
    Command::new("cmd.exe").arg("/c").arg(format!("echo {} && pause", msg)).status().unwrap();
    exit(1);
}

fn main() {
    let args: Vec<String> = env::args().collect();

    // Get Hollow App Directory
    let working_path = env::current_dir().unwrap();
    let root_directory = working_path.join("hollow_app");

    if !root_directory.exists() {
        panic_then_pause(format!("{:?} does not exist", root_directory));
    } else if !root_directory.is_dir() {
        panic_then_pause(format!("{:?} is not a directory", root_directory));
    }

    let executable = root_directory.join("Hollow.Windows.exe");
    if !executable.exists() {
        panic_then_pause(format!("{:?} not found", executable));
    }

    let mut cmd = Command::new(executable);
    cmd.current_dir(root_directory);

    if args.len() > 1 {
        cmd.args(&args[1..]);
    }

    cmd.spawn().unwrap();
}


