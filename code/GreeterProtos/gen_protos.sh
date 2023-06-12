#!/usr/bin/env bash
# Generates the .cs files from the proto files.

# Exit on error.
set -eu


##TODO add to build file

readonly PROTO_EXTENSION=".exe"

readonly proto_tools=${HOME}/.nuget/packages/grpc.tools/1.0.1/tools/windows_x64/

readonly greeter_protos_dir=$(dirname $0)/
readonly generated_dir=${greeter_protos_dir}/generated/



rm -rf  ${generated_dir}/*
mkdir -p ${generated_dir}

PROTOS=$( find ${greeter_protos_dir} -type f -name '*.proto' )

echo PROTOS $PROTOS

for PROTO in $PROTOS ; do

    echo PROTO $PROTO
	
    ${proto_tools}/protoc${PROTO_EXTENSION} $PROTO \
        --csharp_out ${generated_dir}              \
        --proto_path ${greeter_protos_dir}


	${proto_tools}/protoc${PROTO_EXTENSION} $PROTO \
        --proto_path ${greeter_protos_dir}       \
        --grpc_out ${generated_dir}              \
        --plugin=protoc-gen-grpc=${proto_tools}/grpc_csharp_plugin${PROTO_EXTENSION}


done 
