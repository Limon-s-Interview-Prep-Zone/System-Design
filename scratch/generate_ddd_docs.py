# -*- coding: utf-8 -*-
"""
Python generator script for Domain-Driven Design (DDD) Documentation & Notebooks.
Generates:
1. Architecture/DDD.md
2. Architecture/DDD-bn.md
3. Architecture/DDD.ipynb
4. Architecture/DDD-bn.ipynb
Conforming strictly to workspace rules, dual-language parity, .NET/C# pattern standards, and SVG diagrams.
"""

import json
import os

WORKSPACE_DIR = r"D:\Interview\System-Design"
ARCH_DIR = os.path.join(WORKSPACE_DIR, "Architecture")

def make_notebook(cells):
    return {
        "cells": cells,
        "metadata": {
            "language_info": {
                "name": "csharp"
            }
        },
        "nbformat": 4,
        "nbformat_minor: 2": None, # just to remember schema
        "nbformat": 4,
        "nbformat_minor": 2
    }

def md_cell(source_lines):
    return {
        "cell_type": "markdown",
        "metadata": {},
        "source": [line + "\n" for line in source_lines]
    }

def code_cell(source_lines):
    return {
        "cell_type": "code",
        "execution_count": None,
        "metadata": {},
        "outputs": [],
        "source": [line + "\n" for line in source_lines]
    }

print("Generator script loaded.")
