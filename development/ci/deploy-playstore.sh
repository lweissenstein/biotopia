#!/usr/bin/env bash

set -euo pipefail

pushd ${CI_PROJECT_DIR}/development/ci

export JSON_KEY_FILE=./fastlane-json-key.json

gem install bundler
bundle install

mkdir -p ./fastlane/metadata/android/en-EN/changelogs
git log -n 1 | tail -n +3 > ./fastlane/metadata/android/en-EN/${CI_PIPELINE_IID}.txt
# echo "no release notes yet" > ./fastlane/metadata/android/en-EN/changelogs/default.txt

echo ${ANDROID_FASTLANE_BASE64} | base64 -d > $JSON_KEY_FILE

bundle exec fastlane supply \
    --track internal \
    --aab "${CI_PROJECT_DIR}/Builds/Android/${BUILD_NAME}.${ANDROID_BUILD_TYPE}" \
    --json-key $JSON_KEY_FILE \
    --package_name "com.abductedrhino.biotopia" \
    --metadata_path ./fastlane/metadata

rm -f $JSON_KEY_FILE

popd
