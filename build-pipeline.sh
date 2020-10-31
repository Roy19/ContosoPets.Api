chmod +x /home/runner/work/ContosoPets.Api/ContosoPets.Api/build.sh
RELEASE_BRANCH=master
echo "Branch: $BRANCH_NAME"
if [ $BRANCH_NAME = $RELEASE_BRANCH ]
    then
        /home/runner/work/ContosoPets.Api/ContosoPets.Api/build.sh --target=CI --Branch-Name=master --API-Key=${{env.NUGET_GPR_KEY}}
    else
        /home/runner/work/ContosoPets.Api/ContosoPets.Api/build.sh --target=CI --Branch-Name=master --Build-Number=${GITHUB_RUN_NUMBER} --API-Key=${{env.NUGET_GPR_KEY}}
fi