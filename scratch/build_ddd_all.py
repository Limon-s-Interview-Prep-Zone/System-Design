# -*- coding: utf-8 -*-
"""
Build script to compile DDD markdown files and notebooks.
"""

import json
import os
import sys

# Ensure scratch directory is in python search path
SCRATCH_DIR = os.path.dirname(os.path.abspath(__file__))
if SCRATCH_DIR not in sys.path:
    sys.path.insert(0, SCRATCH_DIR)

from content_en import EN_SECTIONS
from content_bn import BN_SECTIONS

WORKSPACE_DIR = r"D:\Interview\System-Design"
ARCH_DIR = os.path.join(WORKSPACE_DIR, "Architecture")

def generate_markdown(sections):
    md_parts = []
    for cell_type, content in sections:
        if cell_type == "markdown":
            md_parts.append(content)
        elif cell_type == "code":
            md_parts.append("```csharp\n" + content + "\n```")
    return "\n\n".join(md_parts)

def generate_notebook(sections):
    cells = []
    for cell_type, content in sections:
        if cell_type == "markdown":
            cells.append({
                "cell_type": "markdown",
                "metadata": {},
                "source": [line + "\n" for line in content.split("\n")]
            })
        elif cell_type == "code":
            cells.append({
                "cell_type": "code",
                "execution_count": None,
                "metadata": {},
                "outputs": [],
                "source": [line + "\n" for line in content.split("\n")]
            })
    return {
        "cells": cells,
        "metadata": {
            "language_info": {
                "name": "csharp"
            }
        },
        "nbformat": 4,
        "nbformat_minor": 2
    }

def main():
    print("Generating files...")

    # 1. English Markdown
    en_md_path = os.path.join(ARCH_DIR, "DDD.md")
    en_md_content = generate_markdown(EN_SECTIONS)
    with open(en_md_path, "w", encoding="utf-8") as f:
        f.write(en_md_content)
    print(f"Written: {en_md_path} ({len(en_md_content)} bytes)")

    # 2. Bangla Markdown
    bn_md_path = os.path.join(ARCH_DIR, "DDD-bn.md")
    bn_md_content = generate_markdown(BN_SECTIONS)
    with open(bn_md_path, "w", encoding="utf-8") as f:
        f.write(bn_md_content)
    print(f"Written: {bn_md_path} ({len(bn_md_content)} bytes)")

    # 3. English Notebook
    en_ipynb_path = os.path.join(ARCH_DIR, "DDD.ipynb")
    en_nb = generate_notebook(EN_SECTIONS)
    with open(en_ipynb_path, "w", encoding="utf-8") as f:
        json.dump(en_nb, f, indent=1, ensure_ascii=False)
    print(f"Written: {en_ipynb_path}")

    # 4. Bangla Notebook
    bn_ipynb_path = os.path.join(ARCH_DIR, "DDD-bn.ipynb")
    bn_nb = generate_notebook(BN_SECTIONS)
    with open(bn_ipynb_path, "w", encoding="utf-8") as f:
        json.dump(bn_nb, f, indent=1, ensure_ascii=False)
    print(f"Written: {bn_ipynb_path}")

    # Validate JSON syntax
    for p in [en_ipynb_path, bn_ipynb_path]:
        with open(p, "r", encoding="utf-8") as f:
            data = json.load(f)
            assert data["nbformat"] == 4
            assert data["nbformat_minor"] == 2
            print(f"Validated JSON integrity for {os.path.basename(p)}: {len(data['cells'])} cells.")

    print("All DDD deliverables successfully generated and validated!")

if __name__ == "__main__":
    main()
