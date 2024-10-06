RED='\033[0;31m'
NC='\033[0m'
BRANCH_NAME="temp-branch"

if git show-ref --verify --quiet refs/heads/$BRANCH_NAME; then
  git checkout $BRANCH_NAME
else
  git checkout -b $BRANCH_NAME
fi
git add .
git commit -m "$1"
git checkout main
git fetch
git pull origin main
git merge temp-branch
echo -e "${RED} NOTE: Remember to delete temp-branch after push ${NC}"
