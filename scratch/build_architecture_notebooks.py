# -*- coding: utf-8 -*-
"""
Build script to generate Architecture.ipynb and Architecture-bn.ipynb
from Architecture.md and Architecture-bn.md.
Conforms strictly to nbformat: 4, nbformat_minor: 2.
"""

import json
import os
import re

WORKSPACE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ARCH_DIR = os.path.join(WORKSPACE_DIR, "Architecture")

def md_to_notebook(md_text):
    # Regex to extract ```csharp ... ``` blocks
    code_pattern = re.compile(r"```csharp\s*\n(.*?)\n```", re.DOTALL)
    
    cells = []
    last_end = 0

    for match in code_pattern.finditer(md_text):
        md_part = md_text[last_end:match.start()].strip()
        code_part = match.group(1)

        if md_part:
            # Further split by top-level section divider "---" if desired, or keep as coherent section blocks
            # Splitting by "---" gives clean logical notebook cells
            sections = re.split(r"\n---\n", md_part)
            for s in sections:
                s_trimmed = s.strip()
                if s_trimmed:
                    lines = [line + "\n" for line in s_trimmed.split("\n")]
                    if lines:
                        lines[-1] = lines[-1].rstrip("\n")
                    cells.append({
                        "cell_type": "markdown",
                        "metadata": {},
                        "source": lines
                    })

        # Add the code cell
        code_lines = [line + "\n" for line in code_part.split("\n")]
        if code_lines:
            code_lines[-1] = code_lines[-1].rstrip("\n")
        cells.append({
            "cell_type": "code",
            "execution_count": None,
            "metadata": {},
            "outputs": [],
            "source": code_lines
        })

        last_end = match.end()

    # Any remaining markdown after the last code block
    remainder = md_text[last_end:].strip()
    if remainder:
        sections = re.split(r"\n---\n", remainder)
        for s in sections:
            s_trimmed = s.strip()
            if s_trimmed:
                lines = [line + "\n" for line in s_trimmed.split("\n")]
                if lines:
                    lines[-1] = lines[-1].rstrip("\n")
                cells.append({
                    "cell_type": "markdown",
                    "metadata": {},
                    "source": lines
                })

    notebook = {
        "cells": cells,
        "metadata": {
            "language_info": {
                "name": "csharp"
            }
        },
        "nbformat": 4,
        "nbformat_minor": 2
    }
    return notebook

def main():
    print("Building Architecture notebooks...")

    # 1. English
    en_md_path = os.path.join(ARCH_DIR, "Architecture.md")
    en_ipynb_path = os.path.join(ARCH_DIR, "Architecture.ipynb")
    with open(en_md_path, "r", encoding="utf-8") as f:
        en_md = f.read()

    en_nb = md_to_notebook(en_md)
    with open(en_ipynb_path, "w", encoding="utf-8") as f:
        json.dump(en_nb, f, indent=1, ensure_ascii=False)
    print(f"Generated {en_ipynb_path} with {len(en_nb['cells'])} cells.")

    # 2. Bangla
    bn_md_path = os.path.join(ARCH_DIR, "Architecture-bn.md")
    bn_ipynb_path = os.path.join(ARCH_DIR, "Architecture-bn.ipynb")
    with open(bn_md_path, "r", encoding="utf-8") as f:
        bn_md = f.read()

    bn_nb = md_to_notebook(bn_md)
    with open(bn_ipynb_path, "w", encoding="utf-8") as f:
        json.dump(bn_nb, f, indent=1, ensure_ascii=False)
    print(f"Generated {bn_ipynb_path} with {len(bn_nb['cells'])} cells.")

if __name__ == "__main__":
    main()

