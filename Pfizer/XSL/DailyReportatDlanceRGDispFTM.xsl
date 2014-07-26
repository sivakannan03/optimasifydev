<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:template match="/"> 
		<div id="tableContainer">
			<table  align="center" bgcolor="white" cellspacing="1" cellpadding="0"  class = "tablebg" isHeader="1" id="tblBrands" consider="1" startRow="1" parameter="DSARHeader">
				<!-- New Code -->	
				<tr class="tableheader" >
							<td colspan="5" align="right" nowrap="nowrap" >Daily Report at a glance (Regular)</td>
						<td colspan="5" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='HDate']/@HeaderDate" />
						</td>
				</tr>
				
				<tr class="tableheader" >
						<td colspan="1" align="right" nowrap="nowrap" >User Name : </td>
						<td colspan="2" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='UName']/@UserName" />
						</td>
						<td colspan="2" align="right" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='ActType']/@VisitType" />
						</td>
						<td colspan="1" align="left" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='FDate']/@FetchDate" />
						</td>
						<td colspan="2" align="right" nowrap="nowrap">Submitted On:</td>
						<td colspan="2" align="right" nowrap="nowrap">
							<xsl:value-of select="//row[@Element='SDate']/@SubmittedDate" />
						</td>
				
				</tr>
				<!-- End -->
		
			<tr class="tableheader" align="center">
					<td class="tableheader" >Joint Work(PSO Name)</td>
					<td class="tableheader" >Focus Doctors</td>					
					<td class="tableheader" >MCL Doctors</td>
					<td class="tableheader" >Specialty</td>
					<td class="tableheader" >Call</td>
					<td class="tableheader" >PCL Doctors</td>
					<td class="tableheader" >MCL Chemist</td>										
					<td class="tableheader" >Call</td>
					<td class="tableheader" >PCL Chemist</td>
					<td class="tableheader" >Project PSO</td>
			</tr>
				 
				<xsl:choose>
					<xsl:when test="count(//row[@Element='DRPSO']/@Mon) &gt;0" >		
						<xsl:apply-templates select="//row[@Element='DRPSO']"/>
					</xsl:when>
					<xsl:otherwise>
						<tr class="tablecontent1"> 
							<td align="center" class="alerttext" colspan="10" >No Records Found !</td>
						</tr>
					</xsl:otherwise>
				</xsl:choose>
			</table>
		</div>
	</xsl:template>

	<xsl:template match="//row[@Element='DRPSO']">		
		<tr class="tablecontent2">			
			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@PSOName" />
			</td>
			
			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@MDrFD" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@MDrName" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@Specialty" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@MCDr" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@PDrName" />
			</td>
			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@MChName" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@MCCh" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@PChName" />
			</td>

			<td align="left" nowrap="nowrap">
				<xsl:value-of select="@ProjectPSO" />
			</td>			
		</tr>
	</xsl:template>

</xsl:stylesheet>