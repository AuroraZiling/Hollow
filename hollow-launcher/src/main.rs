use std::env;
use std::io::{BufRead, BufReader};
use std::path::{Path, PathBuf};
use std::process::{Command, exit, Stdio};
use clap::Parser;

#[derive(Parser)]
#[command(long_about = None)]
struct Cli {
    #[arg(short, long, value_name = "HOLLOW_ROOT_DIRECTORY")]
    root: Option<String>,

    #[arg(long)]
    console: bool,
}

fn panic_then_pause(msg: String) {
    Command::new("cmd.exe").arg("/c").arg(format!("echo {} && pause", msg)).status().unwrap();
    exit(1);
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let cli = Cli::parse();


    if env::consts::OS != "windows" {
        panic!("Support Windows only currently");
    }

    // Get Hollow App Directory
    let working_path = env::current_dir()?;
    let mut root_directory = working_path;
    if let Some(root) = cli.root {
        let root = Path::new(&root);
        if root.is_relative() {
            root_directory = root_directory.join(root);
        } else if root.is_absolute() {
            root_directory = PathBuf::from(root);
        }
    } else {
        root_directory = root_directory.join("hollow_app");
    }
    if !root_directory.exists() {
        panic_then_pause(format!("{:?} does not exist", root_directory));
    } else if !root_directory.is_dir() {
        panic_then_pause(format!("{:?} is not a directory", root_directory));
    }

    let executable = root_directory.join("Hollow.exe");
    if !executable.exists() {
        panic_then_pause(format!("{:?} not found", executable));
    }

    let mut cmd = Command::new(executable);
    cmd.current_dir(root_directory);

    let mut child = cmd
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .spawn()
        .expect("Failed to start process");

    if cli.console {
        println!("Hollow Launcher");

        if let Some(stdout) = child.stdout.take() {
            let reader = BufReader::new(stdout);
            for line in reader.lines() {
                match line {
                    Ok(line) => println!("{}", line),
                    Err(err) => eprintln!("{}", err),
                }
            }
        }
        if let Some(stderr) = child.stderr.take() {
            let reader = BufReader::new(stderr);
            for line in reader.lines() {
                match line {
                    Ok(line) => println!("{}", line),
                    Err(err) => eprintln!("{}", err),
                }
            }
        }
        let status = child.wait().expect("Failed to wait on Hollow");
        println!("Process exited with: {}", status);
    }
    Ok(())
}


