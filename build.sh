#!/usr/bin/env bash

set -e

function compile {
	# Flags used here, not in `make html`:
	#  -n   Run in nit-picky mode. Currently, this generates warnings for all missing references.
	#  -W   Turn warnings into errors. This means that the build stops at the first warning and sphinx-build exits with exit status 1.

	sphinx-build -nW -b html -d docs/_build/doctrees docs docs/_build/html
}

function install {
	pip install -q -r requirements.txt
}

function default {
	install
	compile
}

target="default"

while [[ $# > 0 ]]; do
	case $1 in
		-t|--target)
			shift
			target=$1
			;;
	esac
	shift
done

echo "Running target '$target'"
$target