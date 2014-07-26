<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:include href="PagingTemplate.xsl" />
<xsl:template match="/">
	<div id="tableContainer" >
		<table id="tblPTR" align="center" width="100%" cellspacing="1" cellpadding="0" class = "inner_tb" border="1">
			<tr class="tableContent1" align="center">
				<th>Date</th>
        <th>Pso Name</th>
        <th>Region Code</th>
				<th>Territory Code</th>
				<th>DoctorName</th>
				<th>Specialization</th>
				<th>ProductName</th>
				<th>ProductPack Name</th>
				<th>Prescription</th>
				<th>Hospital</th>
				<th>Stockist</th>
				<th>BVO Status</th>
			</tr>			
				
			<xsl:choose>
						<xsl:when test ="count(//row[(@date)]) &gt;0" >		
							<xsl:apply-templates select="//row[(@date)]"/>
						</xsl:when>
						<xsl:otherwise>
							<tr class="tablecontent1"> 
							<td align="center" class="alerttext" colspan="12" >No Records Found</td>
						</tr>
						</xsl:otherwise>
							
			</xsl:choose>
		</table>
	</div>

	</xsl:template>

	<xsl:template match="//row">
		<tr class="tb-r1">
			<td class="text1">
				<xsl:value-of select="@date" />
			</td>
			
			<td class="text1">
				<xsl:value-of select="@PsoName"/>
			</td>				
			<td class="text1">
				<xsl:value-of select="@RegionCode" />
			</td>
			
			<td class="text1">
				<xsl:value-of select="@TerCode"/>
			</td>				
			<td class="text1">
				<xsl:value-of select="@DoctorName" />
			</td>
			
			<td class="text1">
				<xsl:value-of select="@Specialization"/>
			</td>				
			<td class="text1">
				<xsl:value-of select="@ProductName"/>
			</td>
			<td class="text1">
				<xsl:value-of select="@Productpack"/>
			</td>
			<td class="text1">
				<xsl:value-of select="@Prescription"/>
			</td>
			<td class="text1">
				<xsl:value-of select="@Hospital"/>
			</td>	
			<td class="text1">
				<xsl:value-of select="@Stockist"/>
			</td>		
			<td class="text1">
				<xsl:value-of select="@BVOFlag"/>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>

  
  