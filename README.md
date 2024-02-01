# VR Shopping for Small Retailers: A Study of Low-Cost 3D Digital Scanning and Performance for Realistic Product Display in Immersive E-Commerce
# Table of Contents
1. [Abstract](#Abstract)
2. [Repository structure](#Repository-structure)
3. [Filenames](#filenames)
4. [Python Notebook link](#google-colab-python-notebook-online)

## Abstract 
Virtual Reality (VR) is and will be a key driver in the evolution of e-commerce, providing an immersive and gamified shopping experience. However, for VR shopping spaces to become a reality, retailers' product catalogues must first be digitised into 3D models. While this may be a simple task for retail giants, it can be a major obstacle for small retailers, whose human and financial resources are often more limited, making them less competitive. Therefore, this paper presents an analysis of low-cost scanning technologies for small business owners to digitise their products and make them available on a VR shopping platform, which aims to help improve the competitiveness of small businesses through VR and Artificial Intelligence (AI). The technologies to be considered are photogrammetry, LiDAR sensors and NeRF. In addition to investigating which technology provides the best visual quality of 3D models based on metrics and quantitative results, these models must also offer good performance in commercial VR headsets. In this way, we also analyse the performance of such models when running on Meta Quest 2, Quest Pro and Quest 3 headsets to determine their feasibility and provide use cases for each type of model from a scalability point of view. Finally, our work describes an model optimisation process that can be automated and integrated into VR-ZOCO. While NeRF and photogrammetry are the technologies that provide better visual quality 3D models for VR spaces, LiDAR sensors are not recommended for product digitisation based on the obtained results. In terms of performance, the paper presents which models are more suitable for both VR spaces with a high number of available products, simulating shopping centre floors, and simple shops with one or a few exhibitors.

## Repository structure
This repository is organizaed in the following thematic directories:
- **Assets**, **Packages**, **PojectSettings**. These directories represents the minimum required for the Unity project in which the scenes for the performance analysis of 3D models generated with Luma AI and Polycam were developed. The **Assets** folder includes the Oculus Integration package (please, note it is not the newer version Meta XR All-in-one package, so be aware), which includes the software components that make it possible to interact with the GameObjects that that contians the 3D models. You can import this porject with Unity Hub by selecting the cloned repository folder.
- **Quality_Metrics**. This directory contains the files generated for acquiring the accuracy values shown in the study (CloudCompare .bin files) as well as for NR-3DQA. The README.md file inside such directory details how the MM-PCQA values were obtained. CMDM and MSDM2 values, as mentioned in the paper, were obtained using the Ubuntu's virtual machine with the [MEEP2 platform](https://github.com/MEPP-team/MEPP2), on which such software must be compiled, built and installed.
- **OVRMetricsTool_Files**. This directory contains the .csv files obtained from the application executions of each scene that performance was measured from.
- **3D_Models_link.txt**. This link redirect to a Google's Drive link to donwaload in .zip format all 3D models obtained from the scanning technologies and those used in the study. Note that some of them contains "manifold" in their names, as the computation made by the MEEP2 software required such type of meshes. Therefore, some 3D models needed such adjustment by filling the hole in theei bottom (as we could not scan the base of the products due to the platform used for placing the objects). It also contains the 3D models in vertex color representation instead of textures, as it was also a requirement for CMDM computation.

## Filenames  
Please, note that there might be filenames with missing translations. Therefore, the Spanish : English translations of the objects scanned is as follows:
- **Burner : Quemador**
- **Octopus teddy : Pulpo de peluche**
- **Trophy : Trofeo**
- **Seta : Mushroom**
- **Cubo : Cube**
- **Esfera : Sphere**
- **Pirámide : Pyramid**
- **Letra : Letter**
- **Zapatillas : Sport shoes**

## Google Colab (Python Notebook online)
In [this link](https://colab.research.google.com/drive/1zfpSnslUi08qnFtEYxzXof_BGuaCzsDI?usp=sharing) you can find the Python Notebook created for analysing the performance data. It requires you to upload the files inside **OVRMetricsTool_Files** directory. Please, note that you need to upload the files of each device separately, except for CPU and GPU levels graph, for which you need to upload the files from all devices. This Notebook also contains the code developed for creating the graphs used in the paper.
