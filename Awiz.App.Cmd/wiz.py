from git import Repo
import os
import sys

def is_git_root():
    try:
        repo = Repo(os.getcwd(), search_parent_directories=True)
        return repo.working_tree_dir == os.getcwd()
    except:
        return False  # Not inside a Git repository

def is_wiz_folder_present():
    current_dir = os.getcwd()
    wiz_lower = ".wiz".lower()

    for entry in os.listdir(current_dir):
        if entry.lower() == wiz_lower and os.path.isdir(os.path.join(current_dir, entry)):
            return True
    return False

def create_wiz_folders():
    current_dir = os.getcwd()
    wiz_folder = os.path.join(current_dir, ".wiz")
    views_folder = os.path.join(wiz_folder, "views")

    # Create .wiz folder if it doesn't exist
    os.makedirs(views_folder, exist_ok=True)


# Main
if len(sys.argv) == 2 and sys.argv[1] == "init":
    if not is_git_root():
        print("'" + os.getcwd() + "' is not the root of a git repo")
    elif is_wiz_folder_present():
        print("A-Wiz is already initialized")
    else:
        create_wiz_folders()

else:
    print("Usage: wiz init")

    
        
