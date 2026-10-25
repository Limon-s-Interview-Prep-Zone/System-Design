# Jupyter Notebook Template & JSON Schema

All generated `.ipynb` files must strictly adhere to Jupyter Notebook schema `nbformat: 4`, `nbformat_minor: 2`.

## Standard Notebook Skeleton

```json
{
 "cells": [
  {
   "cell_type": "markdown",
   "metadata": {},
   "source": [
    "# <Topic Title> (English or বাংলা)\n",
    "\n",
    "## 1. Overview\n",
    "Conceptual explanation...\n",
    "\n",
    "![Architecture Diagram](images/<diagram-name>.svg)\n"
   ]
  },
  {
   "cell_type": "markdown",
   "metadata": {},
   "source": [
    "## 2. Architectural Pattern: <Pattern Name>\n",
    "\n",
    "### Benefits\n",
    "- Point 1\n",
    "- Point 2\n",
    "\n",
    "### Drawbacks & Trade-offs\n",
    "- Trade-off 1\n",
    "- Trade-off 2\n",
    "\n",
    "### Target Technology & NuGet Packages (.NET / C#)\n",
    "- `Polly` / `Microsoft.Extensions.Http.Resilience`\n"
   ]
  },
  {
   "cell_type": "code",
   "execution_count": null,
   "metadata": {},
   "outputs": [],
   "source": [
    "// Production-grade C# code example\n",
    "builder.Services.AddHttpClient(\"BackendClient\")\n",
    "    .AddStandardResilienceHandler();\n"
   ]
  }
 ],
 "metadata": {
  "language_info": {
   "name": "csharp"
  }
 },
 "nbformat": 4,
 "nbformat_minor": 2
}
```

