# osx unity build shell script

# $1    UNITY_VERSION
unityversion=$1
srcdir=$2 # srpt

editorlogpath="$(pwd)/Editor.log"
editorpath="/Applications/Unity/Hub/Editor/$unityversion/Unity.app"

# remove previous Editor.log
echo "removing previous editor log..."
[ -f $editorlogpath ] && rm $editorlogpath

echo "starting build using unity v$unityversion..."

unitypid=-1

# launch unity in batch mode
launch_unity () {
    open -g $editorpath --args \
        -batchmode \
        -quit \
        -nographics \
        -developmentBuild \
        -executeMethod "Ktyl.Util.BuildCommand.Run" \
        -logFile $editorlogpath \
        -projectPath "$(pwd)/$srcdir"

        unitypid=`pgrep -n Unity`
        echo "launched unity ($unitypid)"
}

launch_unity

while [ ! -f $editorlogpath ]
do
    if ps -p $unitypid > /dev/null
    then
        echo "waiting for unity ($unitypid)"
    else
        echo "unity is no longer running - trying again"

        # reset launch attempts
        launch_unity
    
        continue
    fi

    # wait a second
    sleep 1
done

# use a safe directory that is automatically removed by the shell when the script exists
work="$(mktemp -d)" || exit $?
trap "rm -rf '$work'" exit

# default exit code
echo 1 > $work/exitcode

# start tail in a subshell and store its pid, then read line by line to determine build result
(tail -n 1 -f $editorlogpath &
    jobs -p %% > "$work/tail.pid"
) | while read line
do
    echo "${line}"

    # check for build success
    if [[ $line == *":: ktyl.build completed with code 0"* ]]; then
        echo 0 > $work/exitcode
    fi

    # check for exit
    # TODO: ideally we should have a better indicator than the package manager's shutdown message
    if [[ $line == *"[Package Manager] Server::Kill -- Server was shutdown"* ]]; then
        break
    fi
done

# kill zombie tail process
kill $(<"$work/tail.pid")

# exit with unity's exit code
exit $(cat $work/exitcode)

# vim: tabstop=4 shiftwidth=4 expandtab


