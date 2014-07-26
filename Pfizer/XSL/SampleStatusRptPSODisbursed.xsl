<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />

<xsl:template match="/">
	<div id="tableContainer">
		<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
			<tr class="tableContent1" align="center">
				<th>Daily Report Date</th>
				<th>Issue Date</th>
				<th>Doctor Name</th>
				<th>Speciality</th>
        <th>Product</th>
				<th>Sample pack</th>
				<th>Qty. Disbursed</th>
			</tr>
			<xsl:choose>
				<xsl:when test="count(//row[@Element='Dis']) &gt;0">
					<xsl:apply-templates select="//row[@Element='Dis']" />				
				</xsl:when>
				<xsl:otherwise>
					<tr class="tablecontent1">
						<td align="center" class="alerttext" colspan="7">
              <span color="red">No  Records Found</span></td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='Dis']">
	<tr class="tablecontent1">
		<td align="left">
			<xsl:value-of select="@DASDDate" />
		</td>
		<td align="left">
			<xsl:value-of select="@DASDCreatedDate" />
		</td>
		<td align="left">
			<xsl:value-of select="@DoctorName" />
		</td>
		<td align="left">
			<xsl:value-of select="@Specialization" />
		</td>
		
		<td align="left">
			<xsl:value-of select="@ProductName" />
		</td>
		<td align="left">
			<xsl:value-of select="@SamplePack" />
		</td>
		<td align="left">
			<xsl:value-of select="@QtyDisbursed" />
		</td>
</tr>
</xsl:template>
</xsl:stylesheet>