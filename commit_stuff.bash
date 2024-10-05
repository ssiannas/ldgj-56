RED='\033[0;31m'

git branch temp-branch
git checkout temp-branch
git add .
git commit -m "$1"
git checkout main
git fetch
git pull origin main
git merge temp-branch
echo -e "${RED} NOTE: Remember to delete temp-branch after push"