# -*- coding: utf-8 -*-
"""
Build script to generate Architecture-bn.ipynb matching Architecture.ipynb cell-for-cell.
Conforms strictly to nbformat: 4, nbformat_minor: 2.
"""

import json
import os

WORKSPACE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ARCH_DIR = os.path.join(WORKSPACE_DIR, "Architecture")

def main():
    en_ipynb_path = os.path.join(ARCH_DIR, "Architecture.ipynb")
    bn_md_path = os.path.join(ARCH_DIR, "Architecture-bn.md")
    bn_ipynb_path = os.path.join(ARCH_DIR, "Architecture-bn.ipynb")

    with open(en_ipynb_path, "r", encoding="utf-8") as f:
        en_nb = json.load(f)

    with open(bn_md_path, "r", encoding="utf-8") as f:
        bn_text = f.read()

    # Extract sections matching each cell
    p0_end = bn_text.find("## ১. আর্কিটেকচারাল")
    cell0 = bn_text[:p0_end].strip()

    cell1 = ""

    p2_end = bn_text.find("## পর্ব ১:")
    cell2 = bn_text[p0_end:p2_end].strip()

    p3_end = bn_text.find("### ১.১ মনোলিথিক")
    cell3 = bn_text[p2_end:p3_end].strip()

    p4_end = bn_text.find("### ১.২ মাইক্রোসার্ভিসেস")
    cell4 = bn_text[p3_end:p4_end].strip()

    p5_code_start = bn_text.find("```csharp", p4_end)
    cell5 = bn_text[p4_end:p5_code_start].strip()
    p5_code_end = bn_text.find("```", p5_code_start + 8) + 3

    cell6_code = en_nb["cells"][6]["source"]

    p7_code_start = bn_text.find("```csharp", p5_code_end)
    cell7 = bn_text[p5_code_end:p7_code_start].strip()
    p7_code_end = bn_text.find("```", p7_code_start + 8) + 3

    cell8_code = en_nb["cells"][8]["source"]

    p9_end = bn_text.find("### ১.৫ সার্ভিস-ওরিয়েন্টেড")
    cell9 = bn_text[p7_code_end:p9_end].strip()

    p10_end = bn_text.find("### ১.৬ সার্ভারলেস")
    cell10 = bn_text[p9_end:p10_end].strip()

    p11_code_start = bn_text.find("```csharp", p10_end)
    cell11 = bn_text[p10_end:p11_code_start].strip()
    p11_code_end = bn_text.find("```", p11_code_start + 8) + 3

    cell12_code = en_nb["cells"][12]["source"]

    p13_end = bn_text.find("## পর্ব ২:")
    cell13 = bn_text[p11_code_end:p13_end].strip()

    p14_end = bn_text.find("### ২.১ লেয়ারড")
    cell14 = bn_text[p13_end:p14_end].strip()

    p15_end = bn_text.find("### ২.২ ক্লিন")
    cell15 = bn_text[p14_end:p15_end].strip()

    p16_end = bn_text.find("### ২.৩ মডুলার")
    cell16 = bn_text[p15_end:p16_end].strip()

    p17_end = bn_text.find("### ২.৪ মাইক্রোকার্নেল")
    cell17 = bn_text[p16_end:p17_end].strip()

    p18_end = bn_text.find("### ২.৫ ইউআই")
    cell18 = bn_text[p17_end:p18_end].strip()

    p19_end = bn_text.find("## পর্ব ৩:")
    cell19 = bn_text[p18_end:p19_end].strip()

    p20_end = bn_text.find("### ৩.১ ইভেন্ট-ড্রিভেন")
    cell20 = bn_text[p19_end:p20_end].strip()

    p21_end = bn_text.find("### ৩.২ সিকিউআরএস")
    cell21 = bn_text[p20_end:p21_end].strip()

    cell22_code = []

    p23_end = bn_text.find("### ৩.৩ ট্রানজ্যাকশনাল")
    cell23 = bn_text[p21_end:p23_end].strip()

    p24_end = bn_text.find("### ৩.৪ ডেটা এক্সেস")
    cell24 = bn_text[p23_end:p24_end].strip()

    p25_end = bn_text.find("## পর্ব ৪:")
    cell25 = bn_text[p24_end:p25_end].strip()

    cell26_code = []

    p27_end = bn_text.find("## পর্ব ৫:")
    cell27 = bn_text[p25_end:p27_end].strip()

    cell28 = bn_text[p27_end:].strip()

    cell29 = ""

    def to_lines(text):
        if not text:
            return []
        lines = [line + "\n" for line in text.split("\n")]
        if lines:
            lines[-1] = lines[-1].rstrip("\n")
        return lines

    bn_cells = [
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell0)},
        {"cell_type": "markdown", "metadata": {}, "source": []},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell2)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell3)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell4)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell5)},
        {"cell_type": "code", "execution_count": None, "metadata": {}, "outputs": [], "source": cell6_code},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell7)},
        {"cell_type": "code", "execution_count": None, "metadata": {}, "outputs": [], "source": cell8_code},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell9)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell10)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell11)},
        {"cell_type": "code", "execution_count": None, "metadata": {}, "outputs": [], "source": cell12_code},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell13)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell14)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell15)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell16)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell17)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell18)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell19)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell20)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell21)},
        {"cell_type": "code", "execution_count": None, "metadata": {}, "outputs": [], "source": []},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell23)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell24)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell25)},
        {"cell_type": "code", "execution_count": None, "metadata": {}, "outputs": [], "source": []},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell27)},
        {"cell_type": "markdown", "metadata": {}, "source": to_lines(cell28)},
        {"cell_type": "markdown", "metadata": {}, "source": []},
    ]

    bn_nb = {
        "cells": bn_cells,
        "metadata": en_nb.get("metadata", {}),
        "nbformat": 4,
        "nbformat_minor": 2
    }

    with open(bn_ipynb_path, "w", encoding="utf-8") as f:
        json.dump(bn_nb, f, indent=1, ensure_ascii=False)

    print(f"Generated {bn_ipynb_path} with {len(bn_nb['cells'])} cells.")

if __name__ == "__main__":
    main()

